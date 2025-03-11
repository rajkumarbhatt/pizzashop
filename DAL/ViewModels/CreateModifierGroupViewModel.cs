using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace DAL.ViewModels
{
    public class CreateModifierGroupViewModel
    {
        public int? ModifierGroupId { get; set; }
        [Required(ErrorMessage = "Modifier Group Name is required")]
        public string ModifierGroupName { get; set; }
        public string? ModifierGroupDescription { get; set; }
        public List<Modifier>? Modifiers { get; set; }
        public List<int>? SelectedModifierIds { get; set; }
    }
}