using DAL.Models;

namespace DAL.ViewModels;

public class AddModifiersModal 
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public List<ModifierGroupsAddItem>? ModifierGroups { get; set; }
}