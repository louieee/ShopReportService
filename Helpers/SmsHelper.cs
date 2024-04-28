using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ReportService.Helpers
{
    public class SmsHelper
    {

        private readonly JsonSerializer _serializer = new();
        private readonly IConfiguration _config;
        private readonly HttpClient client;

        public SmsHelper(IConfiguration config)
        {
            _config = config;
            client = new HttpClient();
            client.BaseAddress = new Uri("https://messaging.supportdom.com/messagingV1/Sms/SendSms");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<bool> SendSms(string sToTelephone, string sMessage, int sPriority)
        {
            try
            {
                var request = new SmsMessage()
                {
                    Telephone = sToTelephone,
                    Message = sMessage,
                    Priority = sPriority
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("ApiKey", _config.GetValue<string>("MSG:ApiKey"));
                client.DefaultRequestHeaders.Add("AppId", _config.GetValue<string>("MSG:AppId"));

                var response = await client.PostAsync(client.BaseAddress, content);
                var str = await response.Content.ReadAsStringAsync();
                await using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                await using var json = new JsonTextReader(reader);
                var jsoncontent = _serializer.Deserialize<SmsResponseBase>(json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendBulkReferalSms(List<string> numbers, string sMessage, int sPriority)
        {
            try
            {

                client.DefaultRequestHeaders.Add("ApiKey", _config.GetValue<string>("MSG:ApiKey"));
                client.DefaultRequestHeaders.Add("AppId", _config.GetValue<string>("MSG:AppId"));

                foreach (var content in numbers.Select(t => new SmsMessage()
                         {
                             Telephone = t,
                             Message = sMessage,
                             Priority = sPriority
                         }).Select(request => new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")))
                {
                    var response = await client.PostAsync(client.BaseAddress, content);
                    var str = await response.Content.ReadAsStringAsync();
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    using var json = new JsonTextReader(reader);
                    var jsoncontent = _serializer.Deserialize<SmsResponseBase>(json);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class SmsMessage
    {
        public string Telephone { get; set; }
        public string Message { get; set; }
        public int Priority { get; set; }
    }
    public class SmsMessageResponse
    {
        public string OrderId { get; set; }
    }
    public class SmsResponseBase
    {
        public SmsMessageResponse Data { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
