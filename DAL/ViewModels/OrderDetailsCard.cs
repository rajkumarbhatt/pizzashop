using DAL.Models;

namespace DAL.ViewModels;

public class OrderDetailsCard 
{
    public string? SectionName { get; set; }
    public string? TableNames { get; set; }
    public List<OrderItemDetials>? OrderItemDetails { get; set; }
    public decimal? SubTotal { get; set; }
    public List<InvoiceTax>? Taxes {get; set; }
    public decimal? TotalPrice {get; set; }
}