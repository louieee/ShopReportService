using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.inventory;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class OrderConsumer
{

    private static OrderPayload?  FormatDataAsOrderPayload(string message)
    {
        OrderPayload? orderPayload = null;
        try
        {
            orderPayload = JsonConvert.DeserializeObject<OrderPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            orderPayload = null;
        }

        return orderPayload;
    }


    public static void HandleOrder(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewOrder(context, data); break;
            case "update":
                handleOrderUpdate(context, data); break;
            case "delete":
                handleOrderDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
        }
    }

    private static void handleNewOrder(DataContext context, string data)
    {
        var orderPayload = FormatDataAsOrderPayload(data);
        if (orderPayload == null) return;
        Console.WriteLine($"New Order: {orderPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var order = new Order
        {
            Id = orderPayload.Id,
            ProductId = orderPayload.ProductId,
            SaleId = orderPayload.SaleId,
            StaffId = orderPayload.StaffId,
            Quantity = orderPayload.Quantity,
            Delivered = orderPayload.Delivered,
            DateDelivered = orderPayload.DateDelivered,
            DateCreated = DateTime.Now
        };
        inventoryRepository.CreateOrder(order);
        

    }
    private static void handleOrderUpdate(DataContext context, string data)
    {
        var orderPayload = FormatDataAsOrderPayload(data);
        if (orderPayload == null) return;
        Console.WriteLine($"New Order: {orderPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var order = new Order
        {
            Id = orderPayload.Id,
            ProductId = orderPayload.ProductId,
            SaleId = orderPayload.SaleId,
            StaffId = orderPayload.StaffId,
            Quantity = orderPayload.Quantity,
            Delivered = orderPayload.Delivered,
            DateDelivered = orderPayload.DateDelivered
        };
        inventoryRepository.UpdateOrder(order);

    }
    private static void handleOrderDelete(DataContext context, string data)
    {
        var orderPayload = FormatDataAsOrderPayload(data);
        if (orderPayload == null) return;
        Console.WriteLine($"New Order: {orderPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var order = new Order
        {
            Id = orderPayload.Id,
            ProductId = orderPayload.ProductId,
            SaleId = orderPayload.SaleId,
            StaffId = orderPayload.StaffId,
            Quantity = orderPayload.Quantity,
            Delivered = orderPayload.Delivered,
            DateDelivered = orderPayload.DateDelivered
        };
        inventoryRepository.DeleteOrder(order);
    }

}