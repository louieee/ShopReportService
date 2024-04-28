namespace ReportService.Responses
{
    public class UserProfileResponse : Response
    {
        public string UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Fullname { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool TelephoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        
        public string OTP { get; set; }
        // public List<RegisteredService> RegisteredServices { get; set; }
        

    }
}
