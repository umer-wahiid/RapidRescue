using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RapidResuce.Enums;

namespace RapidResuce.Models
{
    public class User : BaseEntity
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [StringLength(25, ErrorMessage = "Max 25 Characters Allowed")]
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        
        public string Image { get; set; } 

        [Required(ErrorMessage = "Enter Your Phone Number(eg.03XXXXXXXXX)")]
        [StringLength(11, ErrorMessage = "Please Enter Valid Phone Number")]
        [RegularExpression("^03[0-9]{9}$", ErrorMessage = "Invalid format. It should start with '03' and have 11 digits.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The email cannot exceed 50 characters.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""':{}|<>])[A-Za-z\d!@#$%^&*(),.?""':{}|<>]{8,20}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } 
        
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        [NotMapped]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; } 
    }
}
