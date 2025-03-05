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

        public List<ModifierGroup> GetModifierGroups()
        {
            return _context.ModifierGroups.Where(m => m.IsDeleted == false).OrderBy(m => m.Id).ToList();
        }

        public List<Modifier> GetModifiers()
        {
            return _context.Modifiers.Where(m => m.IsDeleted == false).OrderBy(m => m.Id).ToList();
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

        public List<Item> GetItemsBasedOnSearch(int categoryId, string searchValue)
        {
            if (searchValue == null)
            {
                return _context.Items.Where(i => i.CategoryId == categoryId && i.IsDeleted == false).OrderBy(i => i.Id).ToList();
            }
            return _context.Items.Where(i => i.CategoryId == categoryId && i.Name.ToLower().Contains(searchValue) && i.IsDeleted == false).OrderBy(i => i.Id).ToList();
        }

        public void UpdateItemAvailability(int itemId, bool isAvailable, int userId)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return;
            }
            item.IsAvailable = isAvailable;
            item.UpdatedBy = userId;
            _context.SaveChanges();
        }

        public IActionResult DeleteItem(int itemId, int userId)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return new JsonResult(new {success = false, message = "Item not found"});
            }
            item.IsDeleted = true;
            item.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new {success = true, message = "Item deleted successfully"});
        }
    }
}