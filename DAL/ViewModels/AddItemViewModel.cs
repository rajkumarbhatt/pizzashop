using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels
{
    public class AddItemViewModel
    {
        public int? Id { get; set; }
        [Required (ErrorMessage = "Category is required")]
        public required int CategoryId { get; set; }
        [Required (ErrorMessage = "Item name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only alphabets and numbers are allowed")]
        public required string ItemName { get; set; }
        [Required (ErrorMessage = "Item type is required")]
        public required string Type { get; set; }
        [Required (ErrorMessage = "Rate is required")]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$", ErrorMessage = "Invalid rate")]
        public decimal Rate { get; set; }
        [Required (ErrorMessage = "Quantity is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid quantity")]
        public int Quantity { get; set; }
        [Required (ErrorMessage = "Unit is required")]
        public required string Unit { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDefaultTaxable { get; set; }
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$", ErrorMessage = "Invalid tax amount")]
        public decimal? TaxPercentage { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphabets and numbers are allowed")]
        public string? ShortCode { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ModifierGroupIds { get; set; }
    }
}