using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class AddModifierViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Modifier name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Modifier name should only contain letters and spaces.")]
        [StringLength(50, ErrorMessage = "Modifier name cannot exceed 50 characters.")]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Rate is required.")]
        [Range(0.01, 999.99, ErrorMessage = "Rate must be between 0.01 and 999.99.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Rate must be a valid decimal number with up to two decimal places.")]
        public required decimal Rate { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 999, ErrorMessage = "Quantity must be a positive integer between 1 and 999.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Quantity must be a valid integer.")]
        public required int Quantity { get; set; }
        [Required(ErrorMessage = "Unit is required.")]
        public required string Unit { get; set; }
        [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters.")]
        public string? Description { get; set; }
        public List<int>? ModifierGroupIds { get; set; }
    }
}