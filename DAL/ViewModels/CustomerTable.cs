namespace DAL.ViewModels
{
    public class CustomerTable
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Date { get; set; }
        public int? TotalOrders { get; set; }
    }
}