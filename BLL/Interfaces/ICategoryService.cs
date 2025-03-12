using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        public List<Category> GetCategories();
        public List<Item> GetItemsBasedOnSearch(int categoryId, string searchValue);
        public List<ModifierGroup> GetModifierGroups();
        public ModifierGroup GetModifierGroup(int modifierGroupId);
        public List<ModifierGroup> GetModifierGroupsFromList(List<int> modifierGroupIds);
        public List<Modifier> GetModifiersFromList(List<int> modifierGroupIds);
        public List<ModifierModifiergroupMapping> GetModifierModifierGroupMappings(List<int> modifierGroupIds);
        public JsonResult AddCategory(string categoryName, string categoryDescription, int userId);
        public JsonResult UpdateCategory(int categoryId, string categoryName, string categoryDescription, int userId);
        public JsonResult DeleteCategory(int categoryId, int userId);
        public void UpdateItemAvailability(int itemId, bool isAvailable, int userId);
        public IActionResult DeleteItem(int itemId, int userId);
        public string AddItem(AddItemViewModel addItemViewModel, int userId);
        public IActionResult UpdateItemModifierGroup(AddItemViewModel addItemViewModel, string itemName, int userId);
        public MenuViewModel GetItemData(int itemId);
        public List<Modifier> GetModifiersBasedOnSearch(int modifierGroupId, string searchValue);
        public int GetModifierGroupId(int itemId);
        public IActionResult DeleteModifier(int modifierId, int userId);
        public IActionResult DeleteModifierGroup(int modifierGroupId, int userId);
        public List<Modifier> GetAllModifiers(string searchValue);
        public JsonResult AddModifierGroup(CreateModifierGroupViewModel createModifierGroupViewModel, int userId);
        public MenuViewModel GetModifierGroupDetails(int modifierGroupId);
        public IActionResult AddModifier(AddModifierViewModel createModifierViewModel, int userId);
        public MenuViewModel GetModifierData(int modifierId, int modifierGroupId);
    }
}