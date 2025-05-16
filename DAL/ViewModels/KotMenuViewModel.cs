using DAL.Models;

namespace DAL.ViewModels
{
    public class KotMenuViewModel
    {
        public List<Category>? Categories { get; set; }
        public List<MenuItemsKot>? MenuItemsKot { get; set; }
        public WaitingListModal? WaitingListModal { get; set; }
        public AddModifiersModal? AddModifiersModal { get; set; }
        public OrderDetailsCard? OrderDetailsCard { get; set; }
        public List<InvoiceTax>? TaxesFees { get; set; }
        public List<ItemTaxes>? ItemTaxes { get; set; }
        public bool? AreItemsAdded { get; set; }
        public string? OrderInstruction { get; set; }
        public PaymentViewModal? PaymentViewModal { get; set; }
    }
}