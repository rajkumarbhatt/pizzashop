using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class AddEditCategoryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}