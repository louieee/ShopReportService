using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReportApp.Data;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public static class Consumer
{
    public const string QueueName = "report_queue";
    public static EventingBasicConsumer? CreateConsumer(IModel? channel, DataContext context)
    {
        if (channel == null)
        {
            return null;
        }
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            // Console.WriteLine($"Received message: {message}");
            var payload = GetMessage(message);
            switch (payload.DataType)
            {
                case "user": 
                    UserConsumer.HandleUser(context, payload.Action, payload.Data); break;
                case "lead":
                    LeadConsumer.HandleLead(context, payload.Action, payload.Data); break;
                case "sale":
                    SaleConsumer.HandleSale(context, payload.Action, payload.Data); break;
                case "order":
                    OrderConsumer.HandleOrder(context, payload.Action, payload.Data); break;
                case "chat":
                    ChatConsumer.HandleChat(context, payload.Action, payload.Data); break;
                case "contact":
                    ContactConsumer.HandleContact(context, payload.Action, payload.Data); break;
                case "product":
                    ProductConsumer.HandleProduct(context, payload.Action, payload.Data); break;
                default: HandleDefault(message);
                    break;
                    
            }
            channel?.BasicAck(ea.DeliveryTag, false);
            // channel?.Close();

        };
        return consumer;
    }

    private static void HandleDefault(string message)
    {
        Console.WriteLine($"Unknown Message:  {message}");
    }

    public static ReportPayload GetMessage(string message)
    {
        ReportPayload? payload = null;
        try
        {
            payload = JsonConvert.DeserializeObject<ReportPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            payload = null;
        }

        return payload;
    }
}