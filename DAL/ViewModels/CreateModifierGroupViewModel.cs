using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace DAL.ViewModels
{
    public class CreateModifierGroupViewModel
    {
        public int? ModifierGroupId { get; set; }
        [Required(ErrorMessage = "Modifier Group Name is required")]
        [StringLength(50, ErrorMessage = "Modifier Group Name cannot be longer than 50 characters")]
        public required string ModifierGroupName { get; set; }
        [StringLength(150, ErrorMessage = "Modifier Group Description cannot be longer than 150 characters")]
        public string? ModifierGroupDescription { get; set; }
        public List<Modifier>? Modifiers { get; set; }
        public List<int>? SelectedModifierIds { get; set; }
    }
}