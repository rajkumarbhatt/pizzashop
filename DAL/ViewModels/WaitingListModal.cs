using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class WaitingListModal
{
    public int Id { get; set; }
    [Required (ErrorMessage = "Name is required")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name should contain only alphabets")]
    public required string Name { get; set; }
    [Required (ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }
    [Required (ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone Number should contain only 10 digits")]
    public required string MobileNumber { get; set; }
    [Required (ErrorMessage = "Number of People is required")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "Number of People should contain only digits")]
    public required int NumberOfPeople { get; set; }
    [Required(ErrorMessage = "Section is required")]
    public int? SectionId { get; set; }
}