using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }
        [MaxLength(10, ErrorMessage = "Phone number must be 10 digits")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public bool Status { get; set; }
        public int? RoleIdRequestedUser { get; set; }
        public string? Username { get; set; }
        public string? ProfileImageURL { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string UsernameRequestedUSer { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public int? RoleId { get; set; }      
    }
}