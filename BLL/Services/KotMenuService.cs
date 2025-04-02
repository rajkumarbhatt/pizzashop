using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2010.CustomUI;

namespace BLL.Services
{
    public class KotMenuService : IKotMenuService
    {
        private readonly PizzaShopContext _context;
        public KotMenuService(PizzaShopContext context)
        {
            _context = context;
        }
        public KotMenuViewModel GetKotMenu()
        {
            List<Category> categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            List<MenuItemsKot> menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true).Select(m => new MenuItemsKot
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryId = m.CategoryId,
                Image = m.ImageUrl,
                ItemType = m.ItemType
            }).ToList();
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                Categories = categories,
                MenuItemsKot = menuItemsKot
            };
            return kotMenuViewModel;
        }

        public KotMenuViewModel GetKotMenuItemsBasedOnCategory(int categoryId)
        {
            if (categoryId == -1)
            {
                return GetKotMenu();
            }
            List<Category> categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            List<MenuItemsKot> menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true && m.CategoryId == categoryId).Select(m => new MenuItemsKot
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                CategoryId = m.CategoryId,
                Image = m.ImageUrl,
                ItemType = m.ItemType
            }).ToList();
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                Categories = categories,
                MenuItemsKot = menuItemsKot
            };
            return kotMenuViewModel;
        }

        public KotMenuViewModel SearchMenuItemsKot(string search, int categoryId)
        {
            if (string.IsNullOrEmpty(search))
            {
                return GetKotMenuItemsBasedOnCategory(categoryId);
            }
            List<MenuItemsKot> menuItemsKot = new List<MenuItemsKot>();
            List<Category> categories = _context.Categories.Where(c => c.IsDeleted == false).ToList();
            if (categoryId == -1)
            {
                menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true && m.Name.ToLower().Contains(search.ToLower())).Select(m => new MenuItemsKot
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    CategoryId = m.CategoryId,
                    Image = m.ImageUrl,
                    ItemType = m.ItemType
                }).ToList();
            }
            else
            {
                menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true && m.CategoryId == categoryId && m.Name.ToLower().Contains(search.ToLower())).Select(m => new MenuItemsKot
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    CategoryId = m.CategoryId,
                    Image = m.ImageUrl,
                    ItemType = m.ItemType
                }).ToList();
            }
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                Categories = categories,
                MenuItemsKot = menuItemsKot
            };
            return kotMenuViewModel;
        }
    }
}