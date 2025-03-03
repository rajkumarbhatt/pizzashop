using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        public List<Category> GetCategories();
        public JsonResult AddCategory(string categoryName, string categoryDescription, int userId);
        public JsonResult UpdateCategory(int categoryId, string categoryName, string categoryDescription, int userId);
        public JsonResult DeleteCategory(int categoryId, int userId);
    }
}