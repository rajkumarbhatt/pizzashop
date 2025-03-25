using DAL.Models;

namespace DAL.ViewModels
{
    public class InvoiceModifiers
    {
        public string? Item { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
    }
}