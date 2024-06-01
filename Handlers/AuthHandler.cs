using System.Security;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ReportApp.Data;
using ReportService.Models;
using ReportService.Responses;

namespace ReportService.Handlers
{
    public class AuthHandler
    {
        private readonly IHttpContextAccessor _httcontext;

        private readonly DataContext _db;

        public AuthHandler(IHttpContextAccessor httcontext, DataContext db)
        {
            _httcontext = httcontext;
            _db = db;
        }

        public bool IsValidUser(string userId)
        {
            try
            {
                var claimsIdentity = _httcontext.HttpContext?.User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.Name);
                if (claim == null)
                {
                    return false;
                }
                return claim.Value == userId;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<User> GetLoggedInUser()
        {
            var claimsIdentity = _httcontext.HttpContext?.User.Identity as ClaimsIdentity;
            var claims = _httcontext.HttpContext?.User.Identities.First().Claims;
            foreach (var claim in claims)
            {
                Console.WriteLine($"Value: {claim.Value}");
                Console.WriteLine($"Type: {claim.Type}");
                Console.WriteLine($"Value Type: {claim.ValueType}\n\n");
            }

            var userClaim = claims.First(x => x.Type == "user").Value;
            var jsonObject = JsonSerializer.Deserialize<Dictionary<string, object>>(userClaim);
            if (jsonObject == null || (jsonObject != null && !jsonObject.ContainsKey("user_id")))
            {
                throw new VerificationException("This identity is invalid");
            } 
            var userId = jsonObject["user_id"].ToString();
            if (userId == null)
            {
                throw new VerificationException("This identity does not exist");
            }

            var user = await _db.Users.FirstAsync(x => x.Id ==  int.Parse(userId));
            if (user is null)
            {
                throw new VerificationException("This identity does not exist");
            }

            return user;
        }



    }
}
