using System.ComponentModel.DataAnnotations;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels
{
    public class AddItemViewModel
    {
        public int? Id { get; set; }
        [Required (ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        [Required (ErrorMessage = "Item name is required")]
        public string ItemName { get; set; }
        [Required (ErrorMessage = "Item type is required")]
        public string Type { get; set; }
        [Required (ErrorMessage = "Rate is required")]
        public decimal Rate { get; set; }
        [Required (ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required (ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDefaultTaxable { get; set; }
        public decimal? TaxPercentage { get; set; }
        public string? ShortCode { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ModifierGroupIds { get; set; }
    }
}