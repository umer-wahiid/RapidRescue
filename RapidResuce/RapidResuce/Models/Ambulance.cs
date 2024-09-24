using RapidResuce.Enums;
using System.ComponentModel;

namespace RapidResuce.Models
{
    public class Ambulance : BaseEntity
    {
        [DisplayName("Vehicle No.")]
        public string VehicleNumber { get; set; }

        public string Image { get; set; }
        
        [DisplayName("Equipment Level")]
        public EquipmentLevel EquipmentLevel { get; set; }
        
        [DisplayName("Latitude")]
        public double? LocationLatitude { get; set; }

        [DisplayName("Longitude")]
        public double? LocationLongitude { get; set; }

        [DisplayName("Driver")]
        public int? DriverId { get; set; }
        
        public User Driver { get; set; }

        [DisplayName("EMT")]
        public int? EmtId { get; set; }

        public User Emt { get; set; }

        public List<EmergencyRequest> EmergencyRequests { get; set; }
    }
}