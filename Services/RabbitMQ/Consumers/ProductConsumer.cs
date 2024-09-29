using Newtonsoft.Json;
using ReportApp.Data;
using ReportApp.Data.Models.inventory;
using ReportApp.Data.Services;
using ReportService.Models;

namespace ReportService.Services.RabbitMQ.Consumers;

public class ProductConsumer
{

    private static ProductPayload?  FormatDataAsProductPayload(string message)
    {
        ProductPayload? productPayload = null;
        try
        {
            productPayload = JsonConvert.DeserializeObject<ProductPayload>(message);

        }
        catch (JsonSerializationException ex)
        {
            Console.Write(ex.Message);
            productPayload = null;
        }

        return productPayload;
    }


    public static void HandleProduct(DataContext context, string action, string data)
    {
        switch (action)
        {
            case "create": 
                handleNewProduct(context, data); break;
            case "update":
                handleProductUpdate(context, data); break;
            case "delete":
                handleProductDelete(context, data); break;
            default:
                Console.WriteLine(data); break;
        }
    }

    private static void handleNewProduct(DataContext context, string data)
    {
        var productPayload = FormatDataAsProductPayload(data);
        if (productPayload == null) return;
        Console.WriteLine($"New Product: {productPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Product = new Product
        {
            Id = productPayload.Id,
            Name = productPayload.Name,
            Brand = productPayload.Brand,
            Inventory = productPayload.Inventory,
            Price = productPayload.Price,
            Quantity = productPayload.Quantity,
            DateAdded = DateTime.Now
        };
        inventoryRepository.CreateProduct(Product);

    }
    private static void handleProductUpdate(DataContext context, string data)
    {
        var productPayload = FormatDataAsProductPayload(data);
        if (productPayload == null) return;
        Console.WriteLine($"New Product: {productPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Product = new Product
        {
            Id = productPayload.Id,
            Name = productPayload.Name,
            Brand = productPayload.Brand,
            Inventory = productPayload.Inventory,
            Price = productPayload.Price,
            Quantity = productPayload.Quantity
        };
        inventoryRepository.UpdateProduct(Product);

    }
    private static void handleProductDelete(DataContext context, string data)
    {
        var productPayload = FormatDataAsProductPayload(data);
        if (productPayload == null) return;
        Console.WriteLine($"New Product: {productPayload.Id}");
        var inventoryRepository = new InventoryRepository(context);
        var Product = new Product
        {
            Id = productPayload.Id,
            Name = productPayload.Name,
            Brand = productPayload.Brand,
            Inventory = productPayload.Inventory,
            Price = productPayload.Price,
            Quantity = productPayload.Quantity
        };
        inventoryRepository.DeleteProduct(Product);
    }

}