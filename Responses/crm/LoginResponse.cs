using ReportApp.Data.Models.crm;
using ReportService.Models;

namespace ReportService.Responses
{
    public class LeadResponseData : Lead
    {
        
        
    }

    public class LeadResponse : Response
    {
        public LeadResponseData Data { get; set; }
    }
}
