using System.Text.Json.Serialization;

namespace ReportApp.Data.Services;

public class UserPayload
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}