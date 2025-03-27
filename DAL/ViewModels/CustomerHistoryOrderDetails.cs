namespace DAL.ViewModels
{
    public class CustomerHistoryOrderDetails
    {
        public string? OrderDate { get; set; }
        public string? OrderType { get; set; }
        public double? OrderAmount { get; set; }
        public string? PaymentType { get; set; }
        public int? NumberOfItems { get; set; }
    }
}