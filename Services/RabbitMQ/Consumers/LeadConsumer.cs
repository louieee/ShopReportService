using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.crm;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class LeadConsumer
{

    private static LeadPayload?  FormatDataAsLeadPayload(string message)
    {
        LeadPayload? leadPayload = null;
        try
        {
            leadPayload = JsonConvert.DeserializeObject<LeadPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            leadPayload = null;
        }

        return leadPayload;
    }


    public static void HandleLead(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewLead(context, data); break;
            case "update":
                handleLeadUpdate(context, data); break;
            case "delete":
                handleLeadDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
        }
    }

    private static void handleNewLead(DataContext context, string data)
    {
        var leadPayload = FormatDataAsLeadPayload(data);
        if (leadPayload == null) return;
        Console.WriteLine($"New Lead: {leadPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var lead = new Lead
        {
            Id = leadPayload.Id,
            Title = leadPayload.Title,
            ContactId = leadPayload.ContactId,
            Company = leadPayload.Company,
            OwnerId = leadPayload.OwnerId,
            Source = leadPayload.Source,
            NurturingStatus = leadPayload.NurturingStatus,
            IsDeal = leadPayload.IsDeal,
            ConversionDate = leadPayload.ConversionDate,
            DateCreated = DateTime.Now

        };
        crmRepository.CreateLead(lead);
        

    }
    private static void handleLeadUpdate(DataContext context, string data)
    {
       var leadPayload = FormatDataAsLeadPayload(data);
        if (leadPayload == null) return;
        Console.WriteLine($"New Lead: {leadPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var lead = new Lead
        {
            Id = leadPayload.Id,
            Title = leadPayload.Title,
            ContactId = leadPayload.ContactId,
            Company = leadPayload.Company,
            OwnerId = leadPayload.OwnerId,
            Source = leadPayload.Source,
            NurturingStatus = leadPayload.NurturingStatus,
            IsDeal = leadPayload.IsDeal,
            ConversionDate = leadPayload.ConversionDate,

        };
        crmRepository.UpdateLead(lead);

    }
    private static void handleLeadDelete(DataContext context, string data)
    {
        var leadPayload = FormatDataAsLeadPayload(data);
        if (leadPayload == null) return;
        Console.WriteLine($"New Lead: {leadPayload.Id}");
        var crmRepository = new CRMRepository(context);
        var lead = new Lead
        {
            Id = leadPayload.Id,
            Title = leadPayload.Title,
            ContactId = leadPayload.ContactId,
            Company = leadPayload.Company,
            OwnerId = leadPayload.OwnerId,
            Source = leadPayload.Source,
            NurturingStatus = leadPayload.NurturingStatus,
            IsDeal = leadPayload.IsDeal,
            ConversionDate = leadPayload.ConversionDate
        };
        crmRepository.DeleteLead(lead);
    }

}