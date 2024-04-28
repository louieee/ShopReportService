namespace ReportService.Responses
{
    public class ActivateUserResponse : Response
    {
        public string OTP { get; set; }
        public bool TelephoneVerified { get; set; }
        public bool EmailVerified { get; set; }
    }
}
