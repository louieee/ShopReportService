using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class UserConsumer
{

    private static UserPayload?  FormatDataAsUserPayload(string message)
    {
        UserPayload? userPayload = null;
        try
        {
            userPayload = JsonConvert.DeserializeObject<UserPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            userPayload = null;
        }

        return userPayload;
    }


    public static void HandleUser(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewUser(context, data); break;
            case "update":
                handleUserUpdate(context, data); break;
            case "delete":
                handleUserDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
                
        }
    }

    private static void handleNewUser(DataContext context, string data)
    {
        var userPayload = FormatDataAsUserPayload(data);
        if (userPayload == null) return;
        Console.WriteLine($"New User: {userPayload.Id}");
        var accountRepository = new AccountRepository(context);
        var user = new User
        {
            Id = userPayload.Id,
            FirstName = userPayload.FirstName,
            LastName = userPayload.LastName,
            Email = userPayload.Email,
            Gender = userPayload.Gender,
            DateOfBirth = DateOnly.Parse(userPayload.DateOfBirth),
        };
        if (userPayload.IsCustomer && userPayload.CustomerId != null){
            user.CustomerId = userPayload.CustomerId;
            var customer = new Customer{
                Id = (int)userPayload.CustomerId,
                DateJoined = DateTime.Now
            };
            accountRepository.CreateUser(user);
            accountRepository.CreateCustomer(customer);
        }
        else if (userPayload.IsStaff && userPayload.StaffId != null){
            user.StaffId = userPayload.StaffId;
            var staff = new Staff
            {
                Id = (int)userPayload.StaffId,
                DateJoined = DateTime.Now
            };
            accountRepository.CreateUser(user);
            accountRepository.CreateStaff(staff);
        }
    }
    private static void handleUserUpdate(DataContext context, string data)
    {
        var userPayload = FormatDataAsUserPayload(data);
        if (userPayload == null) return;
        Console.WriteLine($"Updated User: {userPayload.FirstName}");
        var accountRepository = new AccountRepository(context);
        var user = new User
        {
            Id = userPayload.Id,
            FirstName = userPayload.FirstName,
            LastName = userPayload.LastName,
            Email = userPayload.Email,
            Gender = userPayload.Gender,
        };
        if (userPayload.IsCustomer && userPayload.CustomerId != null){
            user.CustomerId = userPayload.CustomerId;
            accountRepository.UpdateUser(user);
        }
        else if (userPayload.IsStaff && userPayload.StaffId != null){
            user.StaffId = userPayload.StaffId;
            accountRepository.UpdateUser(user);
        }
        var userRepository = new AccountRepository(context);

        userRepository.UpdateUser(user);

    }
    private static void handleUserDelete(DataContext context, string data)
    {
        var userPayload = FormatDataAsUserPayload(data);
        var accountRepository = new AccountRepository(context);
        if (userPayload == null) return;
        
        Console.WriteLine($"Deleted User: {userPayload.Id}");
        var user = new User
        {
            Id = userPayload.Id,
            FirstName = userPayload.FirstName,
            LastName = userPayload.LastName,
            Email = userPayload.Email,
            Gender = userPayload.Gender,
            DateOfBirth = DateOnly.Parse(userPayload.DateOfBirth),
        };
        if (userPayload.IsCustomer && userPayload.CustomerId != null){
            user.CustomerId = userPayload.CustomerId;
            var customer = new Customer{
                Id = (int)userPayload.CustomerId,
                DateJoined = DateTime.Now
            };
            accountRepository.DeleteUser(user);
            accountRepository.DeleteCustomer(customer);
        }
        else if (userPayload.IsStaff && userPayload.StaffId != null){
            user.StaffId = userPayload.StaffId;
            var staff = new Staff
            {
                Id = (int)userPayload.StaffId,
                DateJoined = DateTime.Now
            };
            accountRepository.DeleteUser(user);
            accountRepository.DeleteStaff(staff);
        }        

    }

}