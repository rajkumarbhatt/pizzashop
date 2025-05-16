namespace DAL.ViewModels;

public class OrderItemDetials
{
    public int? OrderItemId { get; set; }
    public string? ItemName { get; set; }
    public int? ItemQuantity { get; set; }
    public decimal? ItemTotalPrice { get; set; }
    public decimal? ModifiersTotalPrice { get; set; }
    public int? ItemId { get; set; }
    public List<ModifierDetails>? Modifiers { get; set; }
    public int? ReadyItemQuantity { get; set; }
}