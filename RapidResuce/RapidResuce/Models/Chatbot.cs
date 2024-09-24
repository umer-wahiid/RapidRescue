namespace RapidResuce.Models
{
    public class Chatbot : BaseEntity
    { 
        public int EmergencyRequestId { get; set; }
        public EmergencyRequest EmergencyRequest { get; set; }

        public string Message { get; set; }
    }
}
