using Microsoft.AspNetCore.SignalR;

namespace ReportService.Hubs;

public class RideHub : Hub
{
    public async Task RequestRide(string origin, string destination, int passengers)
    {
        // Simulate finding a driver (replace with actual logic)
        string driverName = "Available Driver";

        // Send notification to driver about the ride request
        await Clients.Group("Drivers").SendAsync("ReceiveRideRequest", origin, destination, passengers);

        // Send confirmation to the passenger (assuming passenger connection ID is available)
        var connectionId = Context.ConnectionId; // Get the connection ID of the passenger
        await Clients.Client(connectionId).SendAsync("RideRequestConfirmed", driverName);
    }

    public override async Task OnConnectedAsync()
    {
        // Add connecting client to the "Drivers" group if their role is "driver"
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return;
        }
        var role = httpContext.Request.Query["role"];
        if (role.ToString().Equals("driver", StringComparison.OrdinalIgnoreCase))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Drivers");
        }

        await base.OnConnectedAsync();
    }
}
