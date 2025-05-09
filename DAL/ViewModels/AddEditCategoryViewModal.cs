using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class AddEditCategoryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only alphabets and numbers are allowed.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        public string? Description { get; set; }
    }
}