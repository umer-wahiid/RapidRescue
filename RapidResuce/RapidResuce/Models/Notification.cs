using RapidResuce.Enums;

namespace RapidResuce.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public UserRole Role { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
