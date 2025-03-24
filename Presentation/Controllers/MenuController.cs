using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class MenuController : Controller
    { 
        private readonly ICategoryService _categoryService;
        private IJwtService _jwtService;
        public MenuController(ICategoryService categoryService, IJwtService jwtService)
        {
            _categoryService = categoryService;
            _jwtService = jwtService;
        }
        public IActionResult Index(int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            int categoryId = categories.FirstOrDefault()?.Id ?? 0;
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue ?? "");
            var modifierGroups = _categoryService.GetModifierGroups();
            var modifierId = modifierGroups.FirstOrDefault()?.Id ?? 0;
            var modifiers = _categoryService.GetModifiersBasedOnSearch(modifierId, searchValue ?? "");
            var pageIndexModifier = 1;
            var pageSizeModifier = 5;
            var totalModifiers = modifiers.Count;
            var allModifiers = _categoryService.GetAllModifiers(searchValue ?? "");
            var TotalPagesModifier = (int)Math.Ceiling(totalModifiers / (double)pageSizeModifier);
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count,
                ModifierGroups = modifierGroups,
                Modifiers = modifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexModifier = pageIndexModifier,
                PageSizeModifier = pageSizeModifier,
                TotalPagesModifier = TotalPagesModifier,
                TotalModifiers = totalModifiers,
                PageIndexAllModifiers = 1,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / (double)pageSize),
                TotalAllModifiers = allModifiers.Count,
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
            return View(menuViewModel);
        }

        [HttpPost]
        public IActionResult AddCategory(string categoryName, string categoryDescription)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.AddCategory(categoryName, categoryDescription, userId);
        }

        [HttpPut]
        public IActionResult UpdateCategory(int categoryId, string categoryName, string categoryDescription)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.UpdateCategory(categoryId, categoryName, categoryDescription, userId);
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int categoryId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteCategory(categoryId, userId);
        }

        [HttpGet]
        public IActionResult ItemsFilter(int pageIndex, int pageSize, int categoryId, string? searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue ?? "");
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count
            };
            return PartialView("_ItemTable", menuViewModel);
        }

        [HttpGet]
        public IActionResult ItemsSearch(int pageIndex, int pageSize, int categoryId, string? searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue ?? "");
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count
            };
            return PartialView("_ItemTable", menuViewModel);
        }

        [HttpPost]
        public void UpdateItemAvailability(int itemId, bool isAvailable)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            _categoryService.UpdateItemAvailability(itemId, isAvailable, userId);
        }

        [HttpDelete]
        public IActionResult DeleteItem(int itemId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteItem(itemId, userId);
        }

        [HttpGet]
        public IActionResult GetModifierGroupData (string modifierGroupIds)
        {

            if (modifierGroupIds == "[]")
            {
                return PartialView("_AddItemModifierGroupPartial", new MenuViewModel());
            }
            var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(modifierGroupIds) ?? new List<ModifierGroupData>();
            var selectedModifierGroups = _categoryService.GetModifierGroupsFromList(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var selectedModifiers = _categoryService.GetModifiersFromList(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var selectedModifierModifierGroupMappings = _categoryService.GetModifierModifierGroupMappings(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var menuViewModel = new MenuViewModel
            {
                SelectedModifierGroups = selectedModifierGroups,
                SelectedModifiers = selectedModifiers,
                SelectedModifierModifierGroupMappings = selectedModifierModifierGroupMappings,
                ModifierGroupData = modifierGroupData ?? new List<ModifierGroupData>()
            };
            return PartialView("_AddItemModifierGroupPartial", menuViewModel);
        }

        [HttpPost]
        public IActionResult AddItem([FromForm]AddItemViewModel addItemViewModel)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return new JsonResult(new { success = false, message = "Invalid input" });  
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            var itemName = _categoryService.AddItem(addItemViewModel, userId);
            if (itemName == null)
            {
                return new JsonResult(new { success = false, message = "Item already exists" });
            }
            return _categoryService.UpdateItemModifierGroup(addItemViewModel, itemName, userId);
        }

        [HttpGet]
        public IActionResult GetItemData(int itemId)
        {
            MenuViewModel menuViewModel = _categoryService.GetItemData(itemId);
            return PartialView("_AddItemPartial", menuViewModel);
        }

        [HttpGet]
        public IActionResult ModifiersFilter(int pageIndex, int pageSize, int modifierGroupId, string? searchValue = null)
        {
            var modifierGroups = _categoryService.GetModifierGroups();
            var modifiers = _categoryService.GetModifiersBasedOnSearch(modifierGroupId, searchValue ?? "");
            var menuViewModel = new MenuViewModel
            {
                ModifierGroups = modifierGroups,
                Modifiers = modifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexModifier = pageIndex,
                PageSizeModifier = pageSize,
                TotalPagesModifier = (int)Math.Ceiling(modifiers.Count / (double)pageSize),
                TotalModifiers = modifiers.Count
            };
            return PartialView("_ModifiersTable", menuViewModel);
        }

        [HttpDelete]
        public IActionResult DeleteModifier(int modifierId, int modifierGroupId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteModifier(modifierId, userId, modifierGroupId);
        }

        [HttpDelete]
        public IActionResult DeleteModifierGroup(int modifierGroupId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteModifierGroup(modifierGroupId, userId);
        }

        [HttpGet]
        public IActionResult AllModifiersFilter(int pageIndex, int pageSize, string? searchValue = null)
        {
            var allModifiers = _categoryService.GetAllModifiers(searchValue ?? "");
            var menuViewModel = new MenuViewModel
            {
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexAllModifiers = pageIndex,
                PageSizeAllModifiers = pageSize,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / (double)pageSize),
                TotalAllModifiers = allModifiers.Count
            };
            return PartialView("_AllModifiersPartial", menuViewModel);
        }

        [HttpPost]
        public IActionResult AddModifierGroup([FromBody]CreateModifierGroupViewModel createModifierGroupViewModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.AddModifierGroup(createModifierGroupViewModel, userId);
        }

        [HttpGet] 
        public IActionResult EditModifierGroup(int modifierGroupId)
        {
            MenuViewModel menuViewModel = _categoryService.GetModifierGroupDetails(modifierGroupId);
            return PartialView("_ModifiersPartial", menuViewModel);
        }

        [HttpPost]
        public IActionResult AddModifier([FromBody]AddModifierViewModel createModifierViewModel)
        {   
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return new JsonResult(new { success = false, message = "Invalid input" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.AddModifier(createModifierViewModel, userId);
        }

        [HttpGet]
        public IActionResult EditModifier(int modifierId, int modifierGroupId)
        {
            MenuViewModel menuViewModel = _categoryService.GetModifierData(modifierId, modifierGroupId);
            return PartialView("_ModifiersPartial", menuViewModel);
        }

        [HttpDelete]
        public IActionResult DeleteSelectedModifiers(List<int> modifierIds, int modifierGroupId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteSelectedModifiers(modifierIds, modifierGroupId, userId);
        }

        [HttpDelete]
        public IActionResult DeleteSelectedItems(List<int> itemIds)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _categoryService.DeleteSelectedItems(itemIds, userId);
        }

        [HttpGet]
        public IActionResult RefreshItemsPartial(int categoryId = -1,int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
             var categories = _categoryService.GetCategories();
            categoryId = categoryId == -1 ? categories.FirstOrDefault()?.Id ?? 0 : categoryId;
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue ?? "");
            var modifierGroups = _categoryService.GetModifierGroups();
            var modifierId = modifierGroups.FirstOrDefault()?.Id ?? 0;
            var modifiers = _categoryService.GetModifiersBasedOnSearch(modifierId, searchValue ?? "");
            var pageIndexModifier = 1;
            var pageSizeModifier = 5;
            var totalModifiers = modifiers.Count;
            var allModifiers = _categoryService.GetAllModifiers(searchValue ?? "");
            var TotalPagesModifier = (int)Math.Ceiling(totalModifiers / (double)pageSizeModifier);
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count,
                ModifierGroups = modifierGroups,
                Modifiers = modifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexModifier = pageIndexModifier,
                PageSizeModifier = pageSizeModifier,
                TotalPagesModifier = TotalPagesModifier,
                TotalModifiers = totalModifiers,
                PageIndexAllModifiers = 1,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / (double)pageSize),
                TotalAllModifiers = allModifiers.Count,
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
            return PartialView("_MenuPartial", menuViewModel);
        }

        [HttpGet]
        public IActionResult RefreshModifiersPartial(int modifierGroupId, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
             var categories = _categoryService.GetCategories();
            int categoryId = categories.FirstOrDefault()?.Id ?? 0;
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue ?? "");
            var modifierGroups = _categoryService.GetModifierGroups();
            var modifierId = modifierGroupId != 0 ? modifierGroupId : modifierGroups.FirstOrDefault()?.Id ?? 0;
            var modifiers = _categoryService.GetModifiersBasedOnSearch(modifierId, searchValue ?? "");
            var pageIndexModifier = 1;
            var pageSizeModifier = 5;
            var totalModifiers = modifiers.Count;
            var allModifiers = _categoryService.GetAllModifiers(searchValue ?? "");
            var TotalPagesModifier = (int)Math.Ceiling(totalModifiers / (double)pageSizeModifier);
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count,
                ModifierGroups = modifierGroups,
                Modifiers = modifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexModifier = pageIndexModifier,
                PageSizeModifier = pageSizeModifier,
                TotalPagesModifier = TotalPagesModifier,
                TotalModifiers = totalModifiers,
                PageIndexAllModifiers = 1,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / (double)pageSize),
                TotalAllModifiers = allModifiers.Count,
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
            return PartialView("_ModifiersPartial", menuViewModel);
        }

        [HttpGet]
        public IActionResult ResetAddItemForm()
        {
            var categories = _categoryService.GetCategories();
            var modifierGroups = _categoryService.GetModifierGroups();
            MenuViewModel menuViewModel = new MenuViewModel
            {
                Categories = categories,
                ModifierGroups = modifierGroups

            };
            return PartialView("_AddItemPartial", menuViewModel);
        }
    }
}