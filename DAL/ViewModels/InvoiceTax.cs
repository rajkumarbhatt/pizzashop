using DAL.Models;

namespace DAL.ViewModels
{
    public class InvoiceTax
    {
        public string? TaxName { get; set; }
        public double TaxAmount { get; set; }
        public string? TaxType { get; set; }
    }
}