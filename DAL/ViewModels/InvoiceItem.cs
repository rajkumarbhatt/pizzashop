using DAL.Models;

namespace DAL.ViewModels
{
    public class InvoiceItem
    {
        public int SrNo { get; set; }
        public string? Item { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public List<InvoiceModifiers>? InvoiceModifiers { get; set; }
    }
}