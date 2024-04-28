namespace ReportService.Responses
{
    public class LoginResponse : Response
    {
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenExpirationDate { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othernames { get; set; }
        public string Fullname { get; set; }
        public string Gender { get; set; }
        public string GpsAddress { get; set; }
        public double GpsLatitude { get; set; }
        public double GpsLongitude { get; set; }
        public string CountryId { get; set; }
        public string IdNumber { get; set; }
        public string Bvn { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool TelephoneVerified { get; set; }
        public bool EmailVerified { get; set; }
        public string UserStatus { get; set; }
        public string UserType { get; set; }
        public bool HasOrganization { get; set; }
        public string OrganizationId { get; set; }
        public bool HasVehicle { get; set; }
        public bool IdVerification { get; set; }
        public bool BvnVerification { get; set; }

    }
}
