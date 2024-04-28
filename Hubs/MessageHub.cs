using Microsoft.AspNetCore.SignalR;

namespace ReportService.Hubs;

public class MessageHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        // Broadcast the message to all connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
