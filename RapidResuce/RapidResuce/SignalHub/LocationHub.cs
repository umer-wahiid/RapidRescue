using Microsoft.AspNetCore.SignalR;
using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Models;

namespace RapidResuce.SignalHub
{
    public class LocationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public LocationHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendLocation(double latitude, double longitude)
        {
            if (Session.Role == UserRole.Driver)
            {
                var ambulance = _context.Ambulances.FirstOrDefault(amb => amb.DriverId == Session.UserId);

                if (ambulance != null)
                    await Clients.All.SendAsync("ReceiveLocation", Session.UserId, ambulance.VehicleNumber, latitude, longitude);
            }
        }
    }
}
