using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class AddModifierViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Modifier name is required.")]
        public required string Name { get; set; }
        
        public required int Rate { get; set; }
        public required int Quantity { get; set; }
        public required string Unit { get; set; }
        public string? Description { get; set; }
        public List<int>? ModifierGroupIds { get; set; }
    }
}