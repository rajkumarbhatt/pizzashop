using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels
{
    public class CreateUserViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]        
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name should contain only alphabets")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        [MinLength(2, ErrorMessage = "First Name should be at least 2 characters long")]
        public required string FirstName { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name should contain only alphabets")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
        public required string Email { get; set; }
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone Number should contain only 10 digits")]
        public string? PhoneNumber { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s,-/]+$", ErrorMessage = "Address should contain only alphabets, numbers, comma, space, hyphen, and front slash")]
        [StringLength(150, ErrorMessage = "Address cannot be longer than 150 characters")]
        public string? Address { get; set; }
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Zip Code should contain only 6 digits")]
        public string? ZipCode { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character")]
        [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters")]
        public required string Password { get; set; }
        public string? Username { get; set; }
        public string? ProfileImageURL { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public required int RoleId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username should contain only alphabets, numbers, underscore and hyphen")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public required string UsernameRequestedUser { get; set; }
        public IFormFile? ProfileImage { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public int RoleIdRequestedUser { get; set; }
    }
}