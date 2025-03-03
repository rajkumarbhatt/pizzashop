using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly PizzaShopContext _context;
        public CategoryService(PizzaShopContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.Where(c => c.IsDeleted == false).OrderBy(c => c.Id).ToList();
        }

        public JsonResult AddCategory(string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new {success = false, message = "User not found"});
            }
            if (_context.Categories.Any(c => c.Name == categoryName))
            {
                return new JsonResult(new {success = false, message = "Category already exists"});
            }
            var category = new Category
            {
                Name = categoryName,
                Description = categoryDescription,
                CreatedBy = userId,
                UpdatedBy = userId
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return new JsonResult(new {success = true, message = "Category added successfully"});
        }

        public JsonResult UpdateCategory(int categoryId, string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new {success = false, message = "User not found"});
            }
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new {success = false, message = "Category not found"});
            }
            if (_context.Categories.Any(c => c.Name == categoryName && c.Id != categoryId))
            {
                return new JsonResult(new {success = false, message = "Category already exists"});
            }
            category.Name = categoryName;
            category.Description = categoryDescription;
            category.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new {success = true, message = "Category updated successfully"});
        }

        public JsonResult DeleteCategory(int categoryId, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new {success = false, message = "User not found"});
            }
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new {success = false, message = "Category not found"});
            }
            category.IsDeleted = true;
            category.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new {success = true, message = "Category deleted successfully"});
        }
    }
}