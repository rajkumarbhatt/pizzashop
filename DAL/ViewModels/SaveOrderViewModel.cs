namespace DAL.ViewModels
{
    public class SaveOrderViewModel
    {
        public int OrderId { get; set; }
        public List<OrderItemSaveOrder>? OrderItems { get; set; }
        public float SubTotal { get; set; }
        public float Total { get; set; }
        public List<InvoiceTax>? OrderTaxes { get; set; }
    }
}