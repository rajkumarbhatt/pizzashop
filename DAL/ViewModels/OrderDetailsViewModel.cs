using DAL.Models;

namespace DAL.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string? PaidOn { get; set; }
        public string? PlacedOn { get; set; }
        public string? ModifiedOn { get; set; }
        public string? OrderDuration { get; set; }
        public string? OrderStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public int? NumberOfPeople { get; set; }
        public List<Table>? Tables { get; set; }
        public string? Section { get; set; }
        public List<InvoiceItem>? InvoiceItems { get; set; }
        public double? SubTotal { get; set; }
        public List<InvoiceTax>? InvoiceTaxes { get; set; }
        public double? Total { get; set; }
        public string? PaymentMethod { get; set; }
    }
}