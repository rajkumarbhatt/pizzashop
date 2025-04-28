using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class AddTaxViewModal
{
    public int TaxId { get; set; }
    [Required(ErrorMessage = "Tax Name is required")]
    [StringLength(50, ErrorMessage = "Tax Name cannot be longer than 50 characters")]
    public string? TaxName { get; set; }
    public string? TaxType { get; set; }
    [Required(ErrorMessage = "Tax Amount is required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Tax Amount")]
    public decimal? TaxAmount { get; set; }
    public bool IsEnabled { get; set; }
}