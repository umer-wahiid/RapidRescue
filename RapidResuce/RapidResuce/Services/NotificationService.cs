using RapidResuce.Context;
using RapidResuce.Enums;
using RapidResuce.Interfaces;
using RapidResuce.Models;

namespace RapidResuce.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SaveNotificationToDatabase(int? userId, UserRole role, string message)
        {
            if (userId.HasValue)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Role = role,
                    Message = message,
                    IsRead = false,
                    CreatedDate = DateTime.Now
                };
                _context.Add(notification);
            }

            _context.SaveChanges();
        }

    }
}
