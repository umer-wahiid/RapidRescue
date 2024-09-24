using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RapidResuce.Models
{
    public class MedicalProfile : BaseEntity
    {
        [DisplayName("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = "Enter Emergency Contact(eg.03XXXXXXXXX)")]
        [StringLength(11, ErrorMessage = "Please Enter Valid Emergency Contact Number")]
        [RegularExpression("^03[0-9]{9}$", ErrorMessage = "Invalid format. It should start with '03' and have 8 digits.")]
        [DisplayName("Emergency Contact")]
        public string EmergencyContact { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Maximum 500 Characters Allowed"), Required]
        [DisplayName("Medical History")]
        public string MedicalHistory { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Maximum 250 Characters Allowed"), Required]
        public string Allergies { get; set; } = string.Empty;
    }
}