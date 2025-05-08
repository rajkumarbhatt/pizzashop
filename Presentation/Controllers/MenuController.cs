using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Presentaion.Controllers
{
    [CustomAuth]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MenuController : Controller
    {
        private readonly ICategoryService _categoryService;
        private IJwtService _jwtService;
        public MenuController(ICategoryService categoryService, IJwtService jwtService)
        {
            _categoryService = categoryService;
            _jwtService = jwtService;
        }
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.GetMenuViewModelAsync(pageIndex, pageSize, searchValue ?? "");
            return View(menuViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(AddEditCategoryViewModel addEditCategoryViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.AddCategoryAsync(addEditCategoryViewModel, userId);
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory(int categoryId)
        {
            MenuViewModel menuViewModel = await _categoryService.GetCategoryDetailsAsync(categoryId);
            return PartialView("_ModalPartial", menuViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteCategoryAsync(categoryId, userId);
        }

        [HttpGet]
        public async Task<IActionResult> ItemsFilter(int pageIndex, int pageSize, int categoryId, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.GetMenuViewModelBasedOnFilterAsync(categoryId, pageIndex, pageSize, searchValue ?? "");
            return PartialView("_ItemTable", menuViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteItemAsync(itemId, userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetModifierGroupData(string modifierGroupIds)
        {
            MenuViewModel menuViewModel = await _categoryService.GetModifierGroupDataAsync(modifierGroupIds);
            return PartialView("_AddItemModifierGroupPartial", menuViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromForm] AddItemViewModel addItemViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            var itemName = await _categoryService.AddItemAsync(addItemViewModel, userId);
            if (itemName == "itemalreadyexists")
            {
                return new JsonResult(new { success = false, message = "Item already exists" });
            }
            if (itemName == "thisisnotacceptable")
            {
                return new JsonResult(new { success = false, message = "Invalid Image" });
            }
            return await _categoryService.UpdateItemModifierGroupAsync(addItemViewModel, itemName, userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemData(int itemId)
        {
            MenuViewModel menuViewModel = await _categoryService.GetItemDataAsync(itemId);
            return PartialView("_AddItemPartial", menuViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ModifiersFilter(int pageIndex, int pageSize, int modifierGroupId, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.ModifiersFilterAsync(pageIndex, pageSize, modifierGroupId, searchValue ?? "");
            return PartialView("_ModifiersTable", menuViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteModifier(int modifierId, int modifierGroupId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteModifierAsync(modifierId, userId, modifierGroupId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteModifierGroup(int modifierGroupId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteModifierGroupAsync(modifierGroupId, userId);
        }

        [HttpGet]
        public async Task<IActionResult> AllModifiersFilter(int pageIndex, int pageSize, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.AllModifiersFilterAsync(pageIndex, pageSize, searchValue ?? "");
            return PartialView("_AllModifiersPartial", menuViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifierGroup([FromBody] CreateModifierGroupViewModel createModifierGroupViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.AddModifierGroupAsync(createModifierGroupViewModel, userId);
        }

        [HttpGet]
        public async Task<IActionResult> EditModifierGroup(int modifierGroupId, int pageIndex, int pageSize)
        {
            MenuViewModel menuViewModel = await _categoryService.GetModifierGroupDetailsAsync(modifierGroupId, pageIndex, pageSize);
            return PartialView("_ModifiersPartial", menuViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifier([FromBody] AddModifierViewModel createModifierViewModel)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.AddModifierAsync(createModifierViewModel, userId);
        }

        [HttpGet]
        public async Task<IActionResult> EditModifier(int modifierId, int modifierGroupId)
        {
            MenuViewModel menuViewModel = await _categoryService.GetModifierDataAsync(modifierId, modifierGroupId);
            return PartialView("_AddEditModifierPartial", menuViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSelectedModifiers(List<int> modifierIds, int modifierGroupId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteSelectedModifiersAsync(modifierIds, modifierGroupId, userId);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSelectedItems(List<int> itemIds)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _categoryService.DeleteSelectedItemsAsync(itemIds, userId);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshItemsPartial(int categoryId = -1, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.RefreshItemsPartialAsync(categoryId, pageIndex, pageSize, searchValue ?? "");
            return PartialView("_MenuPartial", menuViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshModifiersPartial(int modifierGroupId, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            MenuViewModel menuViewModel = await _categoryService.RefreshModifiersPartialAsync(modifierGroupId, pageIndex, pageSize, searchValue ?? "");
            return PartialView("_ModifiersPartial", menuViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ResetAddItemForm()
        {
            MenuViewModel menuViewModel = await _categoryService.ResetAddItemFormAsync();
            return PartialView("_AddItemPartial", menuViewModel);
        }
    }
}