using ReportApp.Data.Models.inventory;
using ReportService.Models;

namespace ReportService.Responses
{
    public class ProductResponseData : Product
    {
        
        
    }

    public class ProductResponse : Response
    {
        public ProductResponseData Data { get; set; }
    }
}
