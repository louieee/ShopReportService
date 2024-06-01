using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReportService.Responses;

namespace ReportService.Helpers
{
    public class TokenHelper
    {
        public static string GenerateAccessToken(LoginResponseData data, IConfiguration configuration)
        {
            var claimsIdentity = new List<Claim>
            {
               new Claim(ClaimTypes.Name, data.Id.ToString())
            };
            var token = GenerateToken(claimsIdentity, DateTime.Now + TimeSpan.FromDays(1), 
                configuration);
            return token;
        }
        
        public static string GenerateRefreshToken(LoginResponseData data, IConfiguration configuration)
        {
            var claimsIdentity = new List<Claim>
            {
                new Claim(ClaimTypes.Name, data.Id.ToString())
            };

            var token = GenerateToken(claimsIdentity, DateTime.Now + TimeSpan.FromDays(3),
                configuration);
            return token;
        }

        private static string GenerateToken(IEnumerable<Claim> claims, DateTime expiry, IConfiguration configuration)
        {
        var  issuer = configuration["Jwt:ValidIssuer"];
        var secret = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException("No JWT Secret"));

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature);

            var tokeOptions = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: expiry,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }


        public static string GetUserIdFromToken(string tokenString, DateTime expiry, IConfiguration configuration)
        {
            var  issuer = configuration["Jwt:ValidIssuer"];
            var secret = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException("No JWT Secret"));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,  // Adjust based on your validation needs
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal validatedToken;

            try
            {
                validatedToken = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out _);
                var expirationClaim = validatedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Expiration);
                if (expirationClaim == null)
                {
                    throw new Exception("Invalid Token");
                }
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;
                if (DateTime.UtcNow > expirationDate)
                {
                    throw new Exception("This token has expired");
                }

            }
            catch (SecurityTokenException ex)
            {
                // Handle token validation exceptions (e.g., invalid signature, expired token)
                throw new Exception("Invalid token", ex);
            }

            var claim = validatedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (claim is null)
            {
                throw new Exception("Invalid Token");
            }

            return claim.Value;
        }
        
        

    }
}
