using DAL.Models;

namespace DAL.ViewModels
{
    public class MenuViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Item> Items { get; set; }
        public List<ModifierGroup> ModifierGroups { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; } 
    }
}