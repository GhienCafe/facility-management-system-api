using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AppCore.Data.Hubs;

public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("test connect");
    }
}