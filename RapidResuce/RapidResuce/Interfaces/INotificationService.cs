using RapidResuce.Enums;

namespace RapidResuce.Interfaces
{
    public interface INotificationService
    {
        void SaveNotificationToDatabase(int? userId, UserRole role, string message);
    }
}
