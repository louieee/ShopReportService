using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.inventory;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class SaleConsumer
{

    private static SalePayload?  FormatDataAsSalePayload(string message)
    {
        SalePayload? salePayload = null;
        try
        {
            salePayload = JsonConvert.DeserializeObject<SalePayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            salePayload = null;
        }

        return salePayload;
    }


    public static void HandleSale(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewSale(context, data); break;
            case "update":
                handleSaleUpdate(context, data); break;
            case "delete":
                handleSaleDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
        }
    }

    private static void handleNewSale(DataContext context, string data)
    {
        var salePayload = FormatDataAsSalePayload(data);
        if (salePayload == null) return;
        Console.WriteLine($"New Sale: {salePayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Sale = new Sale
        {
            Id = salePayload.Id,
            Paid = salePayload.Paid,
            CustomerId = salePayload.CustomerId,
            DateOrdered = salePayload.DateOrdered,
            DatePaid = salePayload.DatePaid,
            Location = salePayload.Location,
            
        };
        inventoryRepository.CreateSale(Sale);

    }
    private static void handleSaleUpdate(DataContext context, string data)
    {
        var salePayload = FormatDataAsSalePayload(data);
        if (salePayload == null) return;
        Console.WriteLine($"New Sale: {salePayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Sale = new Sale
        {
            Id = salePayload.Id,
            Paid = salePayload.Paid,
            CustomerId = salePayload.CustomerId,
            DateOrdered = salePayload.DateOrdered,
            DatePaid = salePayload.DatePaid,
            Location = salePayload.Location,
            
        };
        inventoryRepository.UpdateSale(Sale);

    }
    private static void handleSaleDelete(DataContext context, string data)
    {
        var salePayload = FormatDataAsSalePayload(data);
        if (salePayload == null) return;
        Console.WriteLine($"New Sale: {salePayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Sale = new Sale
        {
            Id = salePayload.Id,
            Paid = salePayload.Paid,
            CustomerId = salePayload.CustomerId,
            DateOrdered = salePayload.DateOrdered,
            DatePaid = salePayload.DatePaid,
            Location = salePayload.Location,
            
        };
        inventoryRepository.DeleteSale(Sale);
    }

}