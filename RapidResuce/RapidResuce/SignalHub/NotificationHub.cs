using Microsoft.AspNetCore.SignalR;
using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Models;
using System.Data;

namespace RapidResuce.SignalHub
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public NotificationHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Session.UserId;
            if (userId != 0)
            {
                // Fetch unread notifications for this user from the database
                var notifications = _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead && n.Role == Session.Role)
                    .Select(n => new { n.Message, n.Id, n.UserId, n.Role })
                    .ToList();


                // Send each notification to the connected user
                foreach (var notification in notifications)
                {
                    //await MarkNotificationAsRead(notification.Id);
                    var test = ((UserRole)notification.Role).ToString();
                    await Clients.Caller.SendAsync("ReceiveNotification", notification.UserId, ((UserRole)notification.Role).ToString(), notification.Message);
                }
            }

            await base.OnConnectedAsync();
        }

        public async Task MarkNotificationAsRead(int? notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }

}