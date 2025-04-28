namespace DAL.ViewModels
{
    using DAL.Models;

    public class KotOrderCardItem
    {
        public int OrderItemId { get; set; }
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public string? ItemInstruction { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemReadyItemsCount { get; set; }
        public List<ModifierDetails>? Modifiers { get; set; }
    }
}