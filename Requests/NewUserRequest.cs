namespace ReportService.Requests
{
    /// <summary>
    /// To be used as Request object for the /User/newuser endpoint
    /// </summary>
    public class NewUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
