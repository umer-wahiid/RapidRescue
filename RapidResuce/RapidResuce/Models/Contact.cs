using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RapidResuce.Models
{
    public class Contact : BaseEntity
    {

        [Required(ErrorMessage = "Enter Your Name")]
        [MaxLength(20, ErrorMessage = "Only 20 Characters are Allowed")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Your Email")]
        [MaxLength(50, ErrorMessage = "Maximum 50 Characters Allowed")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please Enter a Valid Email Address")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Maximum 250 Characters Allowed"), Required]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Maximum 250 Characters Allowed"), Required]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Your Phone Number(eg.03XXXXXXXXX)")]
        [StringLength(11, ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression("^03[0-9]{9}$", ErrorMessage = "Invalid format. It should start with '03' and have 8 digits.")]
        [DisplayName("Phone")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
