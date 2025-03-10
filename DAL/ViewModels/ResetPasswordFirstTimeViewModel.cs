using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class ResetPasswordFirstTimeViewModel
    {

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, one special character and at least 8 characters long")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}