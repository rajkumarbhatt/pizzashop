using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public ModifierGroup GetModifierGroup(int modifierGroupId)
        {
            return _context.ModifierGroups.FirstOrDefault(m => m.Id == modifierGroupId);
        }

        public List<ModifierGroup> GetModifierGroupsFromList(List<int> modifierGroupIds)
        {
            return _context.ModifierGroups.Where(m => modifierGroupIds.Contains(m.Id)).ToList();
        }

        public List<Modifier> GetModifiersFromList(List<int> modifierGroupIds)
        {
            List<Modifier> selectedModifiers = _context.ModifierModifiergroupMappings.Where(m => modifierGroupIds.Contains(m.ModifiergroupId)).Select(m => m.Modifier).ToList();

            return selectedModifiers;
        }

        public List<ModifierModifiergroupMapping> GetModifierModifierGroupMappings(List<int> modifierGroupIds)
        {
            return _context.ModifierModifiergroupMappings.Where(m => modifierGroupIds.Contains(m.ModifiergroupId)).ToList();
        }

        public JsonResult AddCategory(string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            if (_context.Categories.Any(c => c.Name == categoryName))
            {
                return new JsonResult(new { success = false, message = "Category already exists" });
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
            return new JsonResult(new { success = true, message = "Category added successfully" });
        }

        public JsonResult UpdateCategory(int categoryId, string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new { success = false, message = "Category not found" });
            }
            if (_context.Categories.Any(c => c.Name == categoryName && c.Id != categoryId))
            {
                return new JsonResult(new { success = false, message = "Category already exists" });
            }
            category.Name = categoryName;
            category.Description = categoryDescription;
            category.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Category updated successfully" });
        }

        public JsonResult DeleteCategory(int categoryId, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new { success = false, message = "Category not found" });
            }
            category.IsDeleted = true;
            category.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Category deleted successfully" });
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
                return new JsonResult(new { success = false, message = "Item not found" });
            }
            item.IsDeleted = true;
            item.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Item deleted successfully" });
        }

        public string AddItem(AddItemViewModel addItemViewModel, int userId)
        {
            var item = new Item
            {
                CategoryId = addItemViewModel.CategoryId,
                Name = addItemViewModel.ItemName,
                ItemType = addItemViewModel.Type,
                Price = addItemViewModel.Rate,
                Quantity = addItemViewModel.Quantity,
                Unit = addItemViewModel.Unit,
                IsAvailable = addItemViewModel.IsAvailable,
                DefaultTax = addItemViewModel.IsDefaultTaxable,
                TaxPercentage = (decimal)addItemViewModel.TaxPercentage,
                ShortCode = addItemViewModel.ShortCode,
                Description = addItemViewModel.Description,
                CreatedBy = userId,
                UpdatedBy = userId
            };

            if (addItemViewModel.Image != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItemViewModel.Image.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    addItemViewModel.Image.CopyTo(fileStream);
                }
                item.ImageUrl = fileName;
            }
            _context.Items.Add(item);
            _context.SaveChanges();
            return item.Name;
        }

        public IActionResult UpdateItemModifierGroup(AddItemViewModel addItemViewModel, string itemName, int userId)
        {
            var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds);
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
            {
                return new JsonResult(new { success = true, message = "Item added suceccefully" });
            }
            var item = _context.Items.FirstOrDefault(i => i.Name == itemName);
            if (item == null)
            {
                return new JsonResult(new { success = false, message = "Item not found" });
            }
            var modifierGroupIds = modifierGroupData.Select(m => m.Id).ToList();
            foreach (var modifierGroupId in modifierGroupIds)
            {
                var itemModifierMapping = new ItemModifiergroup
                {
                    ItemId = item.Id,
                    ModifiergroupId = modifierGroupId,
                    MinValue = (short?)modifierGroupData.FirstOrDefault(m => m.Id == modifierGroupId).MinimumQuantity,
                    MaxValue = (short?)modifierGroupData.FirstOrDefault(m => m.Id == modifierGroupId).MaximumQuantity,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                _context.ItemModifiergroups.Add(itemModifierMapping);
                _context.SaveChanges();
            }
            return new JsonResult(new { success = true, message = "Item added successfully" });
        }

        public MenuViewModel GetItemData(int itemId)
        {
            var item = _context.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                var menuViewModel1 = new MenuViewModel();
                return menuViewModel1;
            }
            var itemModifierGroups = _context.ItemModifiergroups.Where(i => i.ItemId == itemId).ToList();
            var modifierGroupData = new List<ModifierGroupData>();
            foreach (var itemModifierGroup in itemModifierGroups)
            {
                var modifierGroup = _context.ModifierGroups.FirstOrDefault(m => m.Id == itemModifierGroup.ModifiergroupId);
                modifierGroupData.Add(new ModifierGroupData
                {
                    Id = modifierGroup.Id,
                    Name = modifierGroup.Name,
                    MinimumQuantity = (int)itemModifierGroup.MinValue,
                    MaximumQuantity = (int)itemModifierGroup.MaxValue
                });
            }
            var addItemViewModel = new AddItemViewModel
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                ItemName = item.Name,
                Type = item.ItemType,
                Rate = item.Price,
                Quantity = (int)item.Quantity,
                Unit = item.Unit,
                IsAvailable = (bool)item.IsAvailable,
                IsDefaultTaxable = (bool)item.DefaultTax,
                TaxPercentage = item.TaxPercentage,
                ShortCode = item.ShortCode,
                Description = item.Description,
                ModifierGroupIds = JsonConvert.SerializeObject(modifierGroupData),

            };
            List<ModifierGroup> modifierGroups = GetModifierGroups();
            List<ModifierGroup> selectedModifierGroups = GetModifierGroupsFromList(modifierGroupData.Select(x => x.Id).ToList());
            List<Modifier> selectedModifiers = GetModifiersFromList(modifierGroupData.Select(x => x.Id).ToList());
            List<Category> categories = GetCategories();
            List<ModifierModifiergroupMapping> selectedModifierModifierGroupMappings = GetModifierModifierGroupMappings(modifierGroupData.Select(x => x.Id).ToList());

            MenuViewModel menuViewModel = new MenuViewModel
            {
                AddItemViewModel = addItemViewModel,
                SelectedModifierGroups = selectedModifierGroups,
                SelectedModifiers = selectedModifiers,
                SelectedModifierModifierGroupMappings = selectedModifierModifierGroupMappings,
                ModifierGroupData = modifierGroupData,
                ModifierGroups = modifierGroups,
                Categories = categories
            };
            return menuViewModel;
        }
    }
}