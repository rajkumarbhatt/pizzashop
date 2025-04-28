using DAL.Models;

namespace DAL.ViewModels
{
    public class TaxAndFeeViewModel
    {
        public List<TaxesFee> Taxes { get; set; }
        public AddTaxViewModal? AddTaxModal { get; set; }
    }
}