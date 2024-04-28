

namespace ReportService.Responses
{
    public class OtpResponse: Response
    {
        public string UserId { get; set; }
        public string CountryId { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
    }
}
