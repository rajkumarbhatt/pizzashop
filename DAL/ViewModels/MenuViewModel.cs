using DAL.Models;

namespace DAL.ViewModels
{
    public class MenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Item> Items { get; set; }
        public List<ModifierGroup> ModifierGroups { get; set; }
        public List<Modifier> Modifiers { get; set; }
        public List<ModifierGroup> SelectedModifierGroups { get; set; }
        public List<Modifier> SelectedModifiers { get; set; }
        public List<ModifierModifiergroupMapping> SelectedModifierModifierGroupMappings { get; set; }
        public List<ModifierGroupData> ModifierGroupData { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; } 
        public AddItemViewModel AddItemViewModel { get; set; }
        public int PageIndexModifier { get; set; }
        public int TotalPagesModifier { get; set; }
        public int PageSizeModifier { get; set; }
        public int TotalModifiers { get; set; }
        public List<Modifier> AllModifiers { get; set; }
        public int PageIndexAllModifiers { get; set; }
        public int TotalPagesAllModifiers { get; set; }
        public int PageSizeAllModifiers { get; set; }
        public int TotalAllModifiers { get; set; }
        public CreateModifierGroupViewModel CreateModifierGroupViewModel { get; set; }
        public AddModifierViewModel AddModifierViewModel { get; set; }
        public AddEditCategoryViewModel? AddEditCategoryViewModal { get; set; }
    }
}