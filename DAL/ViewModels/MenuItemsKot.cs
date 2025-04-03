namespace DAL.ViewModels;

public class MenuItemsKot
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int CategoryId { get; set; }
    public string? ItemType { get; set; }
    public bool? IsFavourite { get; set; }
}