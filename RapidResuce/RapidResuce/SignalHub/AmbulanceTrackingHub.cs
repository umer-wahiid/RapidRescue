using Microsoft.AspNetCore.SignalR;

namespace RapidResuce.SignalHub
{
    public class AmbulanceTrackingHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Automatically add a client to a group (e.g., Dispatchers)
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dispatchers");
            await base.OnConnectedAsync();
        }

        // Sends location updates to all connected clients
        public async Task SendLocationUpdate(string ambulanceId, double latitude, double longitude)
        {
            await Clients.All.SendAsync("ReceiveLocationUpdate", ambulanceId, latitude, longitude);
        }

        // Optionally: Send to specific groups (e.g., dispatchers only)
        public async Task JoinDispatcherGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dispatchers");
        }
    }
}
