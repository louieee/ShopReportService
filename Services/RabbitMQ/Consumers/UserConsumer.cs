using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public static class UserConsumer
{
    public const string QueueName = "task";
    public static EventingBasicConsumer? CreateConsumer(IModel? channel)
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
            Console.WriteLine($"Received message: {message}");
            var payload = GetMessage(message);
            switch (payload.Type)
            {
                case "new user": HandleCreate(message);
                    break;
                default: HandleDefault(message);
                    break;
                    
            }
            channel?.BasicAck(ea.DeliveryTag, false);
            // channel?.Close();

        };
        return consumer;
    }

    private static void HandleCreate(string message)
    {
        var user = FormatMessageAsUser(message);
        if (user != null)
        {
            Console.WriteLine($"New User: {user.Id}");
        }
        
    }

    private static void HandleDefault(string message)
    {
        Console.WriteLine($"Unknown Message:  {message}");
    }

    private static string FormatMessageAsString(string message)
    {
        return $"Message: {message}";
    }
    private static User?  FormatMessageAsUser(string message)
    {
        User? user = null;
        try
        {
            user = JsonConvert.DeserializeObject<User>(message);

        }
        catch (JsonSerializationException ex)
        {
            user = null;
        }

        return user;
    }

    private static UserPayload GetMessage(string message)
    {
        UserPayload? payload = null;
        try
        {
            payload = JsonConvert.DeserializeObject<UserPayload>(message);
            
        }
        catch (JsonSerializationException ex)
        {
            payload = null;
        }

        return payload;
    }
}