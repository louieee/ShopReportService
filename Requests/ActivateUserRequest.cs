namespace ReportService.Requests
{
    public class ActivateUserRequest
    {
        public string UserId { get; set; }
        public string OTP { get; set; }
    }
}
