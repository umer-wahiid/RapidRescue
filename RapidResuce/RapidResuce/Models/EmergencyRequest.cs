using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RapidResuce.Enums;

namespace RapidResuce.Models
{
    public class EmergencyRequest : BaseEntity
    {
        [DisplayName("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [DisplayName("Symptoms")]
        public int FirstAidId { get; set; }

        public FirstAid FirstAid { get; set; }

        [DisplayName("Ambulance")]
        public int? AmbulanceId { get; set; } // Initially null until ambulance is assigned

        public Ambulance Ambulance { get; set; }

        public RequestStatus Status { get; set; }

        [DisplayName("Equipment Level")]
        public EquipmentLevel EquipmentLevel { get; set; }

        public RequestType Type { get; set; }

        [Required]
        [DisplayName("Pickup Address")]
        public string PickupAddress { get; set; }

        [Required]
        [DisplayName("Hospital Name")]
        public string HospitalName { get; set; }

        [Required]
        [DisplayName("Emergency Contact")]
        public string EmergencyContact { get; set; }

        [DisplayName("Medical Information")]
        public string MedicalInfo { get; set; }
    }
}
