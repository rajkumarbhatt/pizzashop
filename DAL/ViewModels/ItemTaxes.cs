namespace DAL.ViewModels
{
    public class ItemTaxes
    {
        public int ItemId { get; set; }
        public bool IsDefault { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal ItemPrice { get; set; }
    }
}