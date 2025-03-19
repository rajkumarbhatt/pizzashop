using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class EditUserViewModel
    {
        public int? idLoggednin { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name should contain only alphabets")]
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; }
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone Number should contain only 10 digits")]
        public string? PhoneNumber { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s,-/]+$", ErrorMessage = "Address should contain only alphabets, numbers, comma, space, hyphen, and front slash")]
        public string? Address { get; set; }
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Zip Code should contain only 6 digits")]
        public string? ZipCode { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public required int RoleIdRequestedUser { get; set; }
        public string? Username { get; set; }
        public string? ProfileImageURL { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username should contain only alphabets, numbers, underscore and hyphen")]
        public required string UsernameRequestedUSer { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public int? RoleId { get; set; }      
    }
}