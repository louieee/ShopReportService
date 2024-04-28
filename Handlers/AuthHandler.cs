using System.Security.Claims;

namespace ReportService.Handlers
{
    public class AuthHandler
    {
        private readonly IHttpContextAccessor _httcontext;

        public AuthHandler(IHttpContextAccessor httcontext)
        {
            _httcontext = httcontext;
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
        public bool IsValidClient()
        {
            try
            {
                var claimsIdentity = _httcontext.HttpContext?.User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst("ClientService");
                return claim != null && bool.Parse(claim.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool IsValidVendor()
        {
            try
            {
                var claimsIdentity = _httcontext.HttpContext?.User.Identity as ClaimsIdentity;
                var claim = claimsIdentity?.FindFirst("VendorService");
                return claim != null && bool.Parse(claim.Value);
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
