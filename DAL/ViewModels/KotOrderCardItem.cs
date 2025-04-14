namespace DAL.ViewModels
{
    using DAL.Models;

    public class KotOrderCardItem
    {
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public string? ItemInstruction { get; set; }
        public int ItemQuantity { get; set; }
        public List<string>? Modifiers { get; set; }
    }
}