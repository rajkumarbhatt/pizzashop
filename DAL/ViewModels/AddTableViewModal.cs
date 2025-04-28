using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class AddTableViewModal
    {
        public int TableId { get; set; }
        [Required(ErrorMessage = "Table Name is required")]
        [StringLength(50, ErrorMessage = "Table Name cannot be longer than 50 characters")]
        public string? TableName { get; set; } 
        public string? TableStatus { get; set; }
        [Required(ErrorMessage = "Table Capacity is required")]
        [Range(1, 100, ErrorMessage = "Table Capacity must be between 1 and 100")]
        public int TableCapacity { get; set; }
        public int SectionId { get; set; }
    }
}