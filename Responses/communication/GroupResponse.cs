using ReportService.Models;

namespace ReportService.Responses
{
    public class GroupResponseData : User
    {
        
        
    }

    public class GroupResponse : Response
    {
        public GroupResponseData Data { get; set; }
    }
}
