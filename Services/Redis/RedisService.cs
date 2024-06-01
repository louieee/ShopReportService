using StackExchange.Redis;

namespace ReportApp.Data.Services.Redis;

public class RedisService
{
    
}



class Program
{
    static void Main(string[] args)
    {
        // Connection string to your Redis server
        string redisConnectionString = "localhost";

        // Connect to Redis
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);

        // Get a reference to the Redis database
        IDatabase db = redis.GetDatabase();

        // Now you can use the 'db' object to interact with Redis
        // For example:
        db.StringSet("mykey", "Hello Redis!");
        string value = db.StringGet("mykey");
        Console.WriteLine(value);  // Output: Hello Redis!
    }
}
