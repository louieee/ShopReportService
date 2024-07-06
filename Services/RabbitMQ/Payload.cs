using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ReportApp.Data.Services;

public class ReportPayload
{
    [JsonProperty("action")]
    public string Action { get; set; }
    
    [JsonProperty("data_type")]
    public string DataType { get; set; }
    
    [JsonProperty("data")]
    public string Data { get; set; }
    
}