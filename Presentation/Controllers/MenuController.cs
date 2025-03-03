using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers
{
    public class MenuController : Controller
    { 
        private readonly ICategoryService _categoryService;
        private IJwtService _jwtService;
        public MenuController(ICategoryService categoryService, IJwtService jwtService)
        {
            _categoryService = categoryService;
            _jwtService = jwtService;
        }
        public IActionResult Index()
        {
            var categories = _categoryService.GetCategories();
            var menuViewModel = new MenuViewModel
            {
                Categories = categories
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
    }
}