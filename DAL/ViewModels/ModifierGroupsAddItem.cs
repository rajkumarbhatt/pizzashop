namespace DAL.ViewModels
{
    public class ModifierGroupsAddItem
    {
        public int ModifierGroupId { get; set; }
        public string? ModifierGroupName { get; set; }
        public List<ModifierGroupItemsAddItem>? ModifierGroupItems { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
    }
}