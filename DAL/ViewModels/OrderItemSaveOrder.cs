namespace DAL.ViewModels;

public class OrderItemSaveOrder
{
    public int? OrderItemId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public List<int>? ModifierIds { get; set; }
}