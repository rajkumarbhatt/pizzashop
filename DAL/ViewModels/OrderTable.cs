namespace DAL.ViewModels
{
    public class OrderTable
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public double AvgRating { get; set; }
        public decimal TotalAmount { get; set; }

    }
}