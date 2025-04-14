namespace DAL.ViewModels
{
    using DAL.Models;

    public class KotOrderCard
    {
        public int OrderId { get; set; }
        public string? OrderDuration { get; set; }
        public string? Section { get; set; }
        public string? Table { get; set; }
        public string? OrderInstruction { get; set; }
        public List<KotOrderCardItem>? OrderItems { get; set; }
    }
}