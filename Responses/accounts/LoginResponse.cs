using ReportService.Models;

namespace ReportService.Responses
{
    public class LoginResponseData : User
    {
        
        
    }

    public class LoginResponse : Response
    {
        public LoginResponseData Data { get; set; }
    }
}
