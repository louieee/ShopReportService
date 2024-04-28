using Newtonsoft.Json;

namespace ReportService.Helpers
{
    public class ErrorDetails
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
