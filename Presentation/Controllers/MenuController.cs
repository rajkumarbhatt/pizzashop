using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index(int pageIndex = 1, int pageSize = 5, string searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            int categoryId = categories.FirstOrDefault().Id;
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue);
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count
            };
            return View(menuViewModel);
        }

        [HttpPost]
        public IActionResult AddCategory(string categoryName, string categoryDescription)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _categoryService.AddCategory(categoryName, categoryDescription, userId);
        }

        [HttpPut]
        public IActionResult UpdateCategory(int categoryId, string categoryName, string categoryDescription)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _categoryService.UpdateCategory(categoryId, categoryName, categoryDescription, userId);
        }

        [HttpDelete]
        public IActionResult DeleteCategory(int categoryId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _categoryService.DeleteCategory(categoryId, userId);
        }

        [HttpGet]
        public IActionResult ItemsFilter(int pageIndex, int pageSize, int categoryId, string searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue);
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count
            };
            return PartialView("_MenuPartial", menuViewModel);
        }

        [HttpGet]
        public IActionResult ItemsSearch(int pageIndex, int pageSize, int categoryId, string searchValue = null)
        {
            var categories = _categoryService.GetCategories();
            var items = _categoryService.GetItemsBasedOnSearch(categoryId, searchValue);
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
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            _categoryService.UpdateItemAvailability(itemId, isAvailable, userId);
        }

        [HttpDelete]
        public IActionResult DeleteItem(int itemId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _categoryService.DeleteItem(itemId, userId);
        }
    }
}