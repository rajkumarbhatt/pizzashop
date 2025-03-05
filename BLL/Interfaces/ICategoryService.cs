using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        public List<Category> GetCategories();
        public List<Item> GetItemsBasedOnSearch(int categoryId, string searchValue);
        public List<ModifierGroup> GetModifierGroups();
        public ModifierGroup GetModifierGroup(int modifierGroupId);
        public JsonResult AddCategory(string categoryName, string categoryDescription, int userId);
        public JsonResult UpdateCategory(int categoryId, string categoryName, string categoryDescription, int userId);
        public JsonResult DeleteCategory(int categoryId, int userId);
        public void UpdateItemAvailability(int itemId, bool isAvailable, int userId);
        public IActionResult DeleteItem(int itemId, int userId);
    }
}