using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ReportService.Helpers
{
    public class MailHelper 
    {
        private JsonSerializer _serializer = new JsonSerializer();
        private IConfiguration _config;
        private HttpClient client;

        public MailHelper(IConfiguration config)
        {
            _config = config;
            client = new HttpClient();
            client.BaseAddress = new Uri("https://messaging.supportdom.com/messagingV1/Mail/SendMail");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SendMail(string sTo, string sSubject, string sHtmlBody, int sPriority)
        {
            try
            {
                var request = new MailMessageRequest()
                {
                    ToMail = sTo,
                    Subject = sSubject,
                    Body = sHtmlBody,
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
                var jsonContent = _serializer.Deserialize<MailResponseBase>(json);
                if(jsonContent != null)
                {
                    return jsonContent.StatusCode == 200;
                }
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Mail Error: {exception.Message}");
                return false;
            }
        }

       
        public class MailMessageRequest
        {
            public string ToMail { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public int Priority { get; set; }
        }
        public class MailMessageResponse
        {
            public string OrderId { get; set; }
        }
        public class MailResponseBase
        {
            public MailMessageResponse Data { get; set; }
            public int StatusCode { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
        }


    }
}
