using DAL.Models;

namespace DAL.ViewModels 
{
    public class KotMenuViewModel
    {
        public List<Category>? Categories { get; set; }
        public List<MenuItemsKot>? MenuItemsKot { get; set; }
    }
}