namespace DAL.ViewModels
{
    public class TableCard
    {
        public int? OrderId { get; set; }
        public int? TableId { get; set; }
        public string? TableName { get; set; }
        public int? TableCapacity { get; set; }
        public string? CurentOrderTime { get; set; }
        public string? TableStatus { get; set; }
        public decimal? OrderTotal { get; set; }
    }
}