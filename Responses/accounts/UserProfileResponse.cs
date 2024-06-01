using ReportService.Models;

namespace ReportService.Responses
{
    public class UserProfileData:User
    {

    }

    public class UserProfileResponse : Response
    {
        public UserProfileData Data { get; set; }
    }
        
}
