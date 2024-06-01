

using System.Runtime.InteropServices.JavaScript;

namespace ReportService.Responses
{
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public JSType.Any Data { get; set; }
       
    }
    
}
