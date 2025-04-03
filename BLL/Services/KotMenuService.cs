using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

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
                ItemType = m.ItemType,
                IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
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
            List<MenuItemsKot> menuItemsKot = new List<MenuItemsKot>();
            if (categoryId == -1)
            {
                return GetKotMenu();
            } 
            else if (categoryId == -2)
            {
                menuItemsKot = _context.CustomerFavourites.Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true).Select(cf => new MenuItemsKot {
                    Id = cf.ItemId,
                    Name = cf.Item.Name,
                    Price = cf.Item.Price,
                    Image = cf.Item.ImageUrl,
                    CategoryId = cf.Item.CategoryId,
                    ItemType = cf.Item.ItemType,
                    IsFavourite = !cf.IsDeleted 
                }).ToList();
            }
            else
            {
                menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true && m.CategoryId == categoryId).Select(m => new MenuItemsKot
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    CategoryId = m.CategoryId,
                    Image = m.ImageUrl,
                    ItemType = m.ItemType,
                    IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
                }).ToList();
            }
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
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
            if (categoryId == -1)
            {
                menuItemsKot = _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true && m.Name.ToLower().Contains(search.ToLower())).Select(m => new MenuItemsKot
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    CategoryId = m.CategoryId,
                    Image = m.ImageUrl,
                    IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null,
                    ItemType = m.ItemType
                }).ToList();
            }
            else if (categoryId == -2)
            {
                menuItemsKot = _context.CustomerFavourites.Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true && cf.Item.Name.ToLower().Contains(search.ToLower())).Select(cf => new MenuItemsKot {
                    Id = cf.ItemId,
                    Name = cf.Item.Name,
                    Price = cf.Item.Price,
                    Image = cf.Item.ImageUrl,
                    CategoryId = cf.Item.CategoryId,
                    ItemType = cf.Item.ItemType,
                    IsFavourite = !cf.IsDeleted
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
                    ItemType = m.ItemType,
                    IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
                }).ToList();
            }
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                MenuItemsKot = menuItemsKot
            };
            return kotMenuViewModel;
        }

        public JsonResult AddToFavourites(int itemId, int userId)
        {
            CustomerFavourite customerFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite {ItemId = -2348};
            if (customerFavourite.ItemId == -2348)
            {
                CustomerFavourite customerFavourite2 = new CustomerFavourite
                {
                    ItemId = itemId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = userId,
                    IsDeleted = false
                };
                _context.CustomerFavourites.Add(customerFavourite2);
                _context.SaveChanges();
            }
            else
            {
                customerFavourite.IsDeleted = false;
                customerFavourite.UpdatedAt = DateTime.Now;
                customerFavourite.UpdatedBy = userId;
                _context.SaveChanges();

            }
            return new JsonResult(new { success = true, message = "Item added to favourites succeswsfully" });
        }

        public JsonResult DeleteFromFavourites(int itemId, int userId)
        {
            CustomerFavourite customerFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite();
            customerFavourite.IsDeleted = true;
            customerFavourite.UpdatedAt = DateTime.Now;
            customerFavourite.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Item removed from favourites succeswsfully" });
        }
    }
}