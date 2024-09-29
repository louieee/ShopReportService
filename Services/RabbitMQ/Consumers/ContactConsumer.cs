using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.crm;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class ContactConsumer
{

    private static ContactPayload?  FormatDataAsContactPayload(string message)
    {
        ContactPayload? contactPayload = null;
        try
        {
            contactPayload = JsonConvert.DeserializeObject<ContactPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            contactPayload = null;
        }

        return contactPayload;
    }


    public static void HandleContact(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewContact(context, data); break;
            case "update":
                handleContactUpdate(context, data); break;
            case "delete":
                handleContactDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
        }
    }

    private static void handleNewContact(DataContext context, string data)
    {
        var contactPayload = FormatDataAsContactPayload(data);
        if (contactPayload == null) return;
        Console.WriteLine($"New Contact: {contactPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var contact = new Contact
        {
            Id = contactPayload.Id,
            Name = contactPayload.Name,
            OwnerId = contactPayload.OwnerId,
            DateCreated = DateTime.Now
        };
        crmRepository.CreateContact(contact);

    }
    private static void handleContactUpdate(DataContext context, string data)
    {
        var contactPayload = FormatDataAsContactPayload(data);
        if (contactPayload == null) return;
        Console.WriteLine($"New Contact: {contactPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var contact = new Contact
        {
            Id = contactPayload.Id,
            Name = contactPayload.Name,
            OwnerId = contactPayload.OwnerId
        };
        crmRepository.UpdateContact(contact);

    }
    private static void handleContactDelete(DataContext context, string data)
    {
        var contactPayload = FormatDataAsContactPayload(data);
        if (contactPayload == null) return;
        Console.WriteLine($"New Contact: {contactPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var contact = new Contact
        {
            Id = contactPayload.Id,
            Name = contactPayload.Name,
            OwnerId = contactPayload.OwnerId
        };
        crmRepository.DeleteContact(contact);
    }

}