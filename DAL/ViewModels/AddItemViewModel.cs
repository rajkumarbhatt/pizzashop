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
        [StringLength (50, ErrorMessage = "Item name cannot be longer than 50 characters")]
        public required string ItemName { get; set; }
        [Required (ErrorMessage = "Item type is required")]
        public required string Type { get; set; }
        [Required (ErrorMessage = "Rate is required")]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$", ErrorMessage = "Invalid rate")]
        [Range (0.01, 1999.99, ErrorMessage = "Rate must be between 0.01 and 1999.99")]
        public decimal Rate { get; set; }
        [Required (ErrorMessage = "Quantity is required")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid quantity")]
        [Range (0, 9999, ErrorMessage = "Quantity must be between 0 and 9999")]
        public int Quantity { get; set; }
        [Required (ErrorMessage = "Unit is required")]
        public required string Unit { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDefaultTaxable { get; set; }
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$", ErrorMessage = "Invalid tax amount")]
        [Range (0.01, 99.99, ErrorMessage = "Tax amount must be between 0.01 and 99.99")]
        public decimal? TaxPercentage { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Only alphabets and numbers are allowed")]
        [StringLength (5, ErrorMessage = "Short code cannot be longer than 5 characters")]
        public string? ShortCode { get; set; }
        [StringLength (150, ErrorMessage = "Description cannot be longer than 150 characters")]
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ModifierGroupIds { get; set; }
    }
}