using Newtonsoft.Json;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class UserConsumer
{

    private static User?  FormatDataAsUser(string message)
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


    public static void HandleUser(string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewUser(data); break;
            case "update":
                handleUserUpdate(data); break;
                
            default:
                Console.WriteLine(data); break;
                
        }
    }

    private static void handleNewUser(string data)
    {
        var user = FormatDataAsUser(data);
        if (user != null)
        {
            Console.WriteLine($"New User: {user.Id}");
        }

    }
    
    private static void handleUserUpdate(string data)
    {
        var user = FormatDataAsUser(data);
        if (user != null)
        {
            Console.WriteLine($"Updated User: {user.FirstName}");
        }

    }
}