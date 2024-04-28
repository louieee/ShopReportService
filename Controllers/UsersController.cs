using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReportApp.Data;
using ReportService.Handlers;
using ReportService.Helpers;
using ReportService.Models;
using ReportService.Requests;
using ReportService.Responses;

namespace ReportService.Controllers
{
    /// <summary>
    /// User Management APIs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class UsersController : ControllerBase
    {
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly DataContext _DbContext;
        private readonly IHttpContextAccessor _HttpContext;
        private readonly IConfiguration _config;
        private AuthHandler _AuthHandler;
        private HttpFetchValueHelper FetchValueHelper;
        MailHelper _MailHelper;


        public UsersController(DataContext context, IHttpContextAccessor httpContext, IWebHostEnvironment environment, IConfiguration config)
        {
            _config = config;
            _WebHostEnvironment = environment;
            _DbContext = context;
            _HttpContext = httpContext;
            _AuthHandler = new AuthHandler(_HttpContext);
            FetchValueHelper = new HttpFetchValueHelper(_HttpContext);
            _MailHelper = new MailHelper(config);
        }

        /// <summary>
        /// Retrieves complete user profile information
        /// </summary>
        /// <remarks>Bearer authentication is suspended for this endpoint for now to enable you test if registrations are successful. Bearer Authentication will be added before GoLive</remarks>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserProfile/{UserId}")]
        [Authorize]
        public async Task<ActionResult<UserProfileResponse>> GetUserProfile(string UserId)
        {
            var response = new UserProfileResponse();

            //check authorization claims
            if (!_AuthHandler.IsValidUser(UserId))
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid user credentials.  Please, login and try again.";
                return Unauthorized(response);
            }

            var userProfile = await _DbContext.UserViews.SingleOrDefaultAsync(x => x.Id == UserId);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid UserId.";
                return Ok(response);
            }

            response.Telephone = userProfile.Telephone;
            response.Email = userProfile.Email;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "User profile retrieved successfully";

            return Ok(response);
        }

        private bool UserProfileExists(string id)
        {
            return _DbContext.Users.Any(e => e.Id == id);
        }

        /// <summary>
        /// Signs in user into the app.
        /// </summary>
        /// <remarks>It returns a token and user profile. Use the token for Bearer authentication to access Authorize APIs</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest? request) 
        {
            var response = new Response();
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Missing login details";
                return Ok(response);
            }
            //validate user credentials
            var userProfile = await _DbContext.Users.SingleOrDefaultAsync(user =>user.Email == request.Email);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid email address";
                return Ok(response);
            }
            var passwordHash = HashingHelper.HashUsingPbkdf2(request.Password, userProfile.PasswordSalt);
            if (userProfile.Password != passwordHash)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid PIN or Password";
                return Ok(response);
            }

            if (!userProfile.IsActive)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "You account is currently Disabled.  Activate your account or contact Customer Care.";
                return Ok(response);
            }

            //create response user profile
            var userResponse = new LoginResponse
            {
                UserId = userProfile.Id,
            };

            //generate token
            var token = await Task.Run(() => TokenHelper.GenerateToken(userResponse));
            var refreshToken = TokenHelper.GenerateRefreshToken();

            userProfile.RefreshToken = refreshToken;
            userProfile.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _DbContext.SaveChangesAsync();

            userResponse.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            userResponse.Message = "Login successful";
            userResponse.Token = token;
            userResponse.RefreshToken = refreshToken;
            userResponse.TokenExpirationDate = DateTime.Now.AddDays(2);
            userResponse.Telephone = userProfile.Telephone;
            userResponse.Email = userProfile.Email;
            userResponse.IsActive = userProfile.IsActive;
            return Ok(userResponse);
        }


        [HttpPost]
        [Route("RefreshToken")]
        public IActionResult Refresh(TokenApiModel? tokenApiModel)
        {
            var response = new Response();
            if (response == null)
            {
                var exception = new ArgumentNullException(nameof(response));
                exception.HelpLink = null;
                exception.HResult = 0;
                exception.Source = null;
                throw exception;
            }

            if (tokenApiModel is null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid client request";

                return Ok(response);
            }

            var accessToken = tokenApiModel.AccessToken;
            var refreshToken = tokenApiModel.RefreshToken;
            if (accessToken is null)
            {
                throw new ValidationException("No Access token was passed");
            }

            var principal = TokenHelper.GetPrincipalFromExpiredToken(accessToken);
            var userId = principal.Identity?.Name; //this is mapped to the Name claim by default

            var user = _DbContext.Users.SingleOrDefault(u => u.Id == userId);
            

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid client request";

                return Ok(response);
            }

            var newAccessToken = TokenHelper.GenerateAccessToken(principal.Claims);
            var newRefreshToken = TokenHelper.GenerateRefreshToken();

            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            user.RefreshToken = newRefreshToken;
            _DbContext.SaveChanges();

            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Status = VarHelper.ResponseStatus.SUCCESS.ToString(),
                Message = "Token refreshed",
             });
        }

        [HttpPost, Authorize]
        [Route("RevokeToken")]
        public async Task<IActionResult> Revoke()
        {
            var response = new Response();
            if (response == null) throw new ArgumentNullException(nameof(response));

            var userId = User.Identity?.Name;

            var user = await _DbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid User request";

                return Ok(response);
            }

            user.RefreshToken = null;

            await _DbContext.SaveChangesAsync();
           
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Token revoked";

            return Ok(response);
        }

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <remarks>It does not require Bearer authentication</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("NewUser")]
        public async Task<ActionResult<NewUserResponse>> NewUser([FromBody]NewUserRequest request)
        {
            var response = new NewUserResponse();
            if (request.Password.Length < 4)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "PIN cannot be less than 4 digits.";
                return Ok(response);
            }
            //check if telephone and email exists
            var userTelephone = await _DbContext.Users.SingleOrDefaultAsync(user => user.Email == request.Email);
            if (userTelephone != null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "This email address has been registered already.  Use the recover password to retrieve your login details.";
                return Ok(response);
            }

            //create user record
            AppHelper appHelper = new AppHelper();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(AppHelper.GetUnique8ByteKey());
            string sSalt = Convert.ToBase64String(plainTextBytes);
            string sHashedPassword = HashingHelper.HashUsingPbkdf2(request.Password, sSalt);
            string sVeriCode = appHelper.GetRandomNumber();

            User user = new User
            {
                Id = appHelper.GetNewUniqueId(),
                Email = request.Email,
                PasswordSalt = sSalt,
                Password = sHashedPassword,
                IsActive = true,
                FirstName = VarHelper.DefaultFirstname,
                LastName = VarHelper.DefaultSurname,
                OTP = sVeriCode
            };

            _DbContext.Users.Add(user);
            try
            {
                await _DbContext.SaveChangesAsync(true);
            }
            catch (DbUpdateException)
            {
                if (UserProfileExists(user.Id))
                {
                    response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                    response.Message = "User profile generation conflict.  Please, try again after 5 minutes or contact customer service.";
                    return Ok(response);
                }
                else
                {
                    throw;
                }
            }

            //set response object values
            response.UserId = user.Id;
            response.Email = user.Email;
            response.OTP = sVeriCode;



            // //send verification SMS
            // var smsHelper = new SmsHelper(_config);
            // var message = "Your one time password is " + sVeriCode;
            // var priority = 1;
            // await smsHelper.SendSms(request.CountryId + request.Telephone, message, priority);


            var mailTo = request.Email;
            if (!string.IsNullOrEmpty(request.Email))
            {
                //send email
                const string mailSubject = "PLUGZ OTP";
                const string htmlTemplateFile = "otp.html";
                const string htmlTemplateFolder = "templates";
                var htmlTemplatePath = Path.Combine(_WebHostEnvironment.WebRootPath, htmlTemplateFolder);
                string body;
                using (var reader = new StreamReader(Path.Combine(htmlTemplatePath, htmlTemplateFile)))
                {
                    body = await reader.ReadToEndAsync();
                }

                body = body.Replace("{CODE}", sVeriCode);
                body = body.Replace("{YEAR}", DateTime.Now.Year.ToString());

                await _MailHelper.SendMail(mailTo, mailSubject, body, 1);
            }
          
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Your registration was successful.";
            return Ok(response);
        }

        ///<summary>
        /// Update User Profile
        ///</summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("UpdateUserProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var response = new UserProfileResponse();

            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.Id))
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid user credentials.  Please, login and try again.";
                return Unauthorized(response);
            }

            //verify UserId
            var userProfile = await _DbContext.Users.FindAsync(request.Id);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid UserId.";
                return Ok(response);
            }

            var user = await _DbContext.Users.SingleOrDefaultAsync(x => x.Id == request.Id);

            user.LastName = request.LastName;
            user.FirstName = request.FirstName;
            
            _DbContext.Users.Update(user);
            await _DbContext.SaveChangesAsync(true);

            //set user info to response object
            response.UserId = user.Id;
            response.Fullname = $"{user.FirstName} {user.LastName}";
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            response.Telephone = user.Telephone;
            response.Email = user.Email;
            response.IsActive = user.IsActive;
            response.TelephoneVerified = user.TelephoneVerified;
            response.EmailVerified = user.EmailVerified;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "User Profile Successfully Updated.";

            return Ok(response);
        }


        /// <summary>
        /// Upload user profile image
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadProfileImage")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage([FromBody] UserImageUploadRequest request)
        {
            var response = new UserImageUploadResponse();

            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.UserId))
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid user credentials.  Please, login and try again.";
                return Ok(response);
            }

            //verify UserId
            var userProfile = await _DbContext.Users.FindAsync(request.UserId);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid UserId.";
                return Ok(response);
            }

            //upload pic if available
            var fileHelper = new FileHelper();
            var sMessage = string.Empty;
            var byteImage = Convert.FromBase64String(request.ImageBase64String);
            if (!fileHelper.IsFileCompliant(byteImage, ref sMessage))
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = sMessage;
                return Ok(response);
            }

            // userProfile.Image = request.ImageBase64String;
            _DbContext.Users.Update(userProfile);
            await _DbContext.SaveChangesAsync(true);

            response.ImageBase64String = request.ImageBase64String;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Image upload successful.";

            return Ok(response);
        }


        /// <summary>
        /// Resends user verification code to Telephone and Email address
        /// </summary>
        /// <remarks>It does not require Bearer authentication</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResendOtp")]
        [Authorize]
        public async Task<IActionResult> ResendOtp([FromBody]OtpRequest request)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }

            var appHelper = new AppHelper();
            var smsHelper = new SmsHelper(_config);
            var response = new OtpResponse();

            //verify UserId
            var userProfile = await _DbContext.Users.SingleOrDefaultAsync(user => user.Id == request.UserId && 
                user.Telephone == request.Telephone);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "This is not your resgistered Telephone number.";
                return Ok(response);
            }

            //send verification SMS
            var sVeriCode = appHelper.GetRandomNumber();

            userProfile.OTP = sVeriCode;

            _DbContext.Update(userProfile);

            await _DbContext.SaveChangesAsync();

            var smsRequest = new SmsRequest
            {
                TelePhone = request.Telephone,
                Message = "Your Verification Code is " + sVeriCode
            };

            var priority = 1;


            await smsHelper.SendSms(smsRequest.TelePhone, smsRequest.Message, priority);

            if (!string.IsNullOrEmpty(userProfile.Email))
            {
                //send email
                string body;
                var mailTo = userProfile.Email;
                const string mailSubject = "PLUGZ OTP";
                const string htmlTemplateFile = "otp.html";
                const string htmlTemplateFolder = "templates";
                var htmlTemplatePath = Path.Combine(_WebHostEnvironment.WebRootPath, htmlTemplateFolder);
                using (var reader = new StreamReader(Path.Combine(htmlTemplatePath, htmlTemplateFile)))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{CODE}", sVeriCode);
                body = body.Replace("{YEAR}", DateTime.Now.Year.ToString());

                await _MailHelper.SendMail(mailTo, mailSubject, body, 1);
            }

            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "OTP code sent successfully.";
            response.OTP = sVeriCode;

            return Ok(response);
        }

        /// <summary>
        /// Activates newly registered user 
        /// </summary>
        /// <remarks>This endpoint should be called after the user enters the verification code sent to his/her Telephone.  It does not require Bearer authentication</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ActivateUser")]
        [Authorize]
        public async Task<ActionResult<ActivateUserResponse>> ActivateUser(ActivateUserRequest request)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }

            ActivateUserResponse response = new ActivateUserResponse();

            //get userprofile using UserId
            var userProfile = await _DbContext.Users.SingleOrDefaultAsync(user => user.Id == request.UserId);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid UserId.";
                return Ok(response);
            }

            if(userProfile.OTP != request.OTP)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Invalid OTP.";
                return Ok(response);
            }

            userProfile.TelephoneVerified = true;
            userProfile.EmailVerified = true;
            

            _DbContext.Users.Update(userProfile);
            try
            {
                await _DbContext.SaveChangesAsync(true);
            }
            catch (DbUpdateException)
            {
                if (UserProfileExists(userProfile.Id))
                {
                    response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                    response.Message = "Unable to modify User profile.  Please, try again after 5 minutes or contact customer service.";
                    return Ok(response);
                }
                else
                {
                    throw;
                }
            }

            //set response object values
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "User activated successfully.";
            response.EmailVerified = userProfile.EmailVerified;
            response.TelephoneVerified = userProfile.TelephoneVerified;

            return Ok(response);
        }


        ///<summary>
        /// Api for Changing Password
        ///</summary>
        /// <param name="request"></param>
        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }

            var response = new Response();

            
            var userProfile = await _DbContext.Users.SingleOrDefaultAsync(x => x.Id == request.UserId);

            var passwordHash = HashingHelper.HashUsingPbkdf2(request.OldPassword, userProfile?.PasswordSalt);
            if (userProfile.Password != passwordHash)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "Old Pin does not match Current Pin.";
                return Ok(response);
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(AppHelper.GetUnique8ByteKey());
            var sSalt = Convert.ToBase64String(plainTextBytes);
            var sHashedPassword = HashingHelper.HashUsingPbkdf2(request.NewPassword, sSalt);

            userProfile.Password = sHashedPassword;
            userProfile.PasswordSalt = sSalt;

            _DbContext.Users.Update(userProfile);

            await _DbContext.SaveChangesAsync();

            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Your Pin was successfully changed.";

            return Ok(response);
        }


        ///<summary>
        /// Api for getting user by phone
        ///</summary>
        /// <param name="telephone"></param>
        [HttpGet]
        [Route("GetUserByTelephone/{telephone}")]
        [Authorize]
        public async Task<IActionResult> GetUserByTelephone(string telephone)
        {
            var response = new UserProfileResponse();

            var userProfile = await _DbContext.UserViews.SingleOrDefaultAsync(x => x.Telephone == telephone);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "UnKnown Telephone.";
                return Ok(response);
            }

           

            //set user info to response object
            response.UserId = userProfile.Id;
            response.Fullname = userProfile.FullName;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "User profile retrieved successfully";

            return Ok(response);
        }


        ///<summary>
        /// Api for getting telephone otp
        ///</summary>
        /// <param name="UserId"></param>
        [HttpGet]
        [Route("ChangeTelephoneOTP/{UserId}")]
        [Authorize]
        public async Task<IActionResult> ChangeTelephoneOTP(string UserId)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }
            var response = new EmailTelephoneResponse();

            var userProfile = await _DbContext.UserViews.SingleOrDefaultAsync(x => x.Id == UserId);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "UnKnown User.";
                return Ok(response);
            }


            var appHelper = new AppHelper();
            var telephoneCode = appHelper.GetRandomNumber();

            //send verification SMS
            var smsHelper = new SmsHelper(_config);
            var message = "Your Telephone verification code is " + telephoneCode;
            const int priority = 1;
            if (await smsHelper.SendSms(userProfile.Telephone, message, priority) == false)
            {
                response.Status = VarHelper.ResponseStatus.PENDING.ToString();
                response.Message = "Error Sending Text Message.";
                return Ok(response);
            }

            //set user info to response object
            response.TelephoneOTP = telephoneCode;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Please check your phone for verification code";

            return Ok(response);
        }


        ///<summary>
        /// Api for gettingemail and telephone otp
        ///</summary>
        /// <param name="UserId"></param>
        [HttpGet]
        [Route("ChangeEmailOTP/{UserId}")]
        [Authorize]
        public async Task<IActionResult> ChangeEmailOTP(string UserId)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }
            var response = new EmailTelephoneResponse();

            var userProfile = await _DbContext.UserViews.SingleOrDefaultAsync(x => x.Id == UserId);
            if (userProfile == null)
            {
                response.Status = VarHelper.ResponseStatus.ERROR.ToString();
                response.Message = "UnKnown User.";
                return Ok(response);
            }


            var appHelper = new AppHelper();

            var emailCode = appHelper.GetRandomNumber();

            

            //send email
            string body;
            var mailTo = userProfile.Email;
            var mailSubject = "PLUGZ OTP";
            var HtmlTemplateFile = "otp.html";
            var HtmlTemplateFolder = "templates";
            var htmlTemplatePath = Path.Combine(_WebHostEnvironment.WebRootPath, HtmlTemplateFolder);
            using (var reader = new StreamReader(Path.Combine(htmlTemplatePath, HtmlTemplateFile)))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{CODE}", emailCode);
            body = body.Replace("{YEAR}", DateTime.Now.Year.ToString());

            await _MailHelper.SendMail(mailTo, mailSubject, body, 1);


            //set user info to response object
            response.EmailOTP = emailCode;
            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Please check your email for verification code";

            return Ok(response);
        }


        ///<summary>
        /// Api for Changing Email and Password
        ///</summary>
        /// <param name="request"></param>
        [Authorize]
        [HttpPost]
        [Route("ChangeEmailTelephoneOTP")]
        public async Task<IActionResult> ChangeEmailTelephoneOTP([FromBody] EmailTelephoneRequest request)
        {
            //check authorization claims
            if (!_AuthHandler.IsValidUser(request.UserId))
            {
                return Ok("Invalid user credentials.  Please, login and try again.");
            }
            var response = new Response();


            var userProfile = await _DbContext.Users.SingleOrDefaultAsync(x => x.Id == request.UserId);
            if (userProfile is null)
            {
                return Ok("Invalid user credentials.");
            }

            userProfile.Telephone = request.Telephone;
            userProfile.Email = request.Email;

            _DbContext.Users.Update(userProfile);

            await _DbContext.SaveChangesAsync();

            response.Status = VarHelper.ResponseStatus.SUCCESS.ToString();
            response.Message = "Your info was successfully changed.";

            return Ok(response);
        }
    }
}
