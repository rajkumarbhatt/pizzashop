using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetCategoriesAsync();
        public Task<List<Item>> GetItemsBasedOnSearchAsync(int categoryId, string searchValue);
        public Task<List<ModifierGroup>> GetModifierGroupsAsync();
        public Task<ModifierGroup> GetModifierGroupAsync(int modifierGroupId);
        public Task<List<ModifierGroup>> GetModifierGroupsFromListAsync(List<int> modifierGroupIds);
        public Task<List<Modifier>> GetModifiersFromListAsync(List<int> modifierGroupIds);
        public Task<List<ModifierModifiergroupMapping>> GetModifierModifierGroupMappingsAsync(List<int> modifierGroupIds);
        public Task<JsonResult> AddCategoryAsync(AddEditCategoryViewModel addEditCategoryViewModel, int userId);
        public Task<MenuViewModel> GetCategoryDetailsAsync(int categoryId);
        public Task<JsonResult> UpdateCategoryAsync(int categoryId, string categoryName, string categoryDescription, int userId);
        public Task<JsonResult> DeleteCategoryAsync(int categoryId, int userId);
        public Task UpdateItemAvailabilityAsync(int itemId, bool isAvailable, int userId);
        public Task<IActionResult> DeleteItemAsync(int itemId, int userId);
        public Task<string> AddItemAsync(AddItemViewModel addItemViewModel, int userId);
        public Task<IActionResult> UpdateItemModifierGroupAsync(AddItemViewModel addItemViewModel, string itemName, int userId);
        public Task<MenuViewModel> GetItemDataAsync(int itemId);
        public Task<List<Modifier>> GetModifiersBasedOnSearchAsync(int modifierGroupId, string searchValue);
        public Task<int> GetModifierGroupIdAsync(int itemId);
        public Task<IActionResult> DeleteModifierAsync(int modifierId, int userId, int modifierGroupId);
        public Task<IActionResult> DeleteModifierGroupAsync(int modifierGroupId, int userId);
        public Task<List<Modifier>> GetAllModifiersAsync(string searchValue);
        public Task<JsonResult> AddModifierGroupAsync(CreateModifierGroupViewModel createModifierGroupViewModel, int userId);
        public Task<MenuViewModel> GetModifierGroupDetailsAsync(int modifierGroupId);
        public Task<IActionResult> AddModifierAsync(AddModifierViewModel createModifierViewModel, int userId);
        public Task<MenuViewModel> GetModifierDataAsync(int modifierId, int modifierGroupId);
        public Task<IActionResult> DeleteSelectedModifiersAsync(List<int> modifierIds, int modifierGroupId, int userId);
        public Task<IActionResult> DeleteSelectedItemsAsync(List<int> itemIds, int userId);
        public Task<MenuViewModel> GetMenuViewModelAsync(int pageIndex, int pageSize, string searchValue);
        public Task<MenuViewModel> GetMenuViewModelBasedOnFilterAsync(int categoryId, int pageIndex = 1, int pageSize = 5, string searchValue = "");
        public Task<MenuViewModel> GetModifierGroupDataAsync(string modifierGroupIds);
        public Task<MenuViewModel> ModifiersFilterAsync(int pageIndex, int pageSize, int modifierGroupId, string? searchValue = null);
        public Task<MenuViewModel> AllModifiersFilterAsync(int pageIndex, int pageSize, string? searchValue = null);
        public Task<MenuViewModel> RefreshItemsPartialAsync(int categoryId = -1, int pageIndex = 1, int pageSize = 5, string? searchValue = null);
        public Task<MenuViewModel> RefreshModifiersPartialAsync(int modifierGroupId, int pageIndex = 1, int pageSize = 5, string? searchValue = null);
        public Task<MenuViewModel> ResetAddItemFormAsync();
    }
}