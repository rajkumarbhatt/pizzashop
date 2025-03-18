using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
namespace DAL.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        public int? RoleId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public int ?CityId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string? ProfileImageURL { get; set; }
        public IFormFile? UserProfileImage { get; set; }
    }
}