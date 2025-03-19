using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public required string Token { get; set; } 

        [Required(ErrorMessage = "Password is required")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}