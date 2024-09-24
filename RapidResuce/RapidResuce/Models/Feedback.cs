using RapidResuce.Enums;
using System.ComponentModel;

namespace RapidResuce.Models
{
    public class Feedback : BaseEntity
    {
        [DisplayName("How was you experience?")]
        public Rating Rating { get; set; }

        [DisplayName("Emergency Request")]
        public int EmergencyRequestId { get; set; }

        public EmergencyRequest EmergencyRequest { get; set; } 
        
        public string Comments { get; set; }
    }
}
