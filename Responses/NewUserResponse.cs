

namespace ReportService.Responses
{
    public class NewUserResponse: Response
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
