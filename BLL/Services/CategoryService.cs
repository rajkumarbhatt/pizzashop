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
            if (addItemViewModel.Id == null)
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
            else
            {
                var item = _context.Items.FirstOrDefault(i => i.Id == addItemViewModel.Id);
                if (item == null)
                {
                    return null;
                }
                item.CategoryId = addItemViewModel.CategoryId;
                item.Name = addItemViewModel.ItemName;
                item.ItemType = addItemViewModel.Type;
                item.Price = addItemViewModel.Rate;
                item.Quantity = addItemViewModel.Quantity;
                item.Unit = addItemViewModel.Unit;
                item.IsAvailable = addItemViewModel.IsAvailable;
                item.DefaultTax = addItemViewModel.IsDefaultTaxable;
                item.TaxPercentage = (decimal)addItemViewModel.TaxPercentage;
                item.ShortCode = addItemViewModel.ShortCode;
                item.Description = addItemViewModel.Description;
                item.UpdatedBy = userId;
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
                _context.SaveChanges();
                return item.Name;
            }
        }


        public IActionResult UpdateItemModifierGroup(AddItemViewModel addItemViewModel, string itemName, int userId)
        {
            if (addItemViewModel.Id == null)
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
            else
            {
                if (addItemViewModel.ModifierGroupIds == null)
                {
                    return new JsonResult(new { success = true, message = "Item updated suceccefully" });
                }
                var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds);
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
                {
                    return new JsonResult(new { success = true, message = "Item updated suceccefully" });
                }
                var item = _context.Items.FirstOrDefault(i => i.Id == addItemViewModel.Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }
                var modifierGroupIds = modifierGroupData.Select(m => m.Id).ToList();
                var itemModifierGroups = _context.ItemModifiergroups.Where(i => i.ItemId == item.Id).ToList();
                foreach (var itemModifierGroup in itemModifierGroups)
                {
                    _context.ItemModifiergroups.Remove(itemModifierGroup);
                    _context.SaveChanges();
                }
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
                return new JsonResult(new { success = true, message = "Item updated successfully" });
            }

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

        public List<Modifier> GetModifiersBasedOnSearch(int modifierGroupId, string searchValue)
        {
            if (searchValue == null)
            {
                return _context.ModifierModifiergroupMappings.Where(m => m.ModifiergroupId == modifierGroupId).Select(m => m.Modifier).Where(m => m.IsDeleted == false).ToList();
            }
            return _context.ModifierModifiergroupMappings.Where(m => m.ModifiergroupId == modifierGroupId && m.Modifier.Name.ToLower().Contains(searchValue)).Select(m => m.Modifier).Where(m => m.IsDeleted == false).ToList();
        }

        public int GetModifierGroupId(int itemId)
        {
            return _context.ModifierModifiergroupMappings.FirstOrDefault(m => m.ModifierId == itemId).ModifiergroupId;
        }

        public IActionResult DeleteModifier(int modifierId, int userId)
        {
            var modifier = _context.Modifiers.FirstOrDefault(m => m.Id == modifierId);
            if (modifier == null)
            {
                return new JsonResult(new { success = false, message = "Modifier not found" });
            }
            modifier.IsDeleted = true;
            modifier.UpdatedBy = userId;
            modifier.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Modifier deleted successfully" });
        }

        public IActionResult DeleteModifierGroup(int modifierGroupId, int userId)
        {
            var modifierGroup = _context.ModifierGroups.FirstOrDefault(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                return new JsonResult(new { success = false, message = "Modifier group not found" });
            }
            modifierGroup.IsDeleted = true;
            modifierGroup.UpdatedBy = userId;
            modifierGroup.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Modifier group deleted successfully" });
        }

        public List<Modifier> GetAllModifiers(string searchValue)
        {
            if (searchValue == null)
            {
                return _context.Modifiers.Where(m => m.IsDeleted == false).ToList();
            }
            return _context.Modifiers.Where(m => m.Name.ToLower().Contains(searchValue) && m.IsDeleted == false).ToList();
        }

        public JsonResult AddModifierGroup(CreateModifierGroupViewModel createModifierGroupViewModel, int userId)
        {
            if (createModifierGroupViewModel.ModifierGroupId == 0)
            {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (_context.ModifierGroups.Any(m => m.Name == createModifierGroupViewModel.ModifierGroupName))
                {
                    return new JsonResult(new { success = false, message = "Modifier group already exists" });
                }
                var modifierGroup = new ModifierGroup
                {
                    Name = createModifierGroupViewModel.ModifierGroupName,
                    Description = createModifierGroupViewModel.ModifierGroupDescription,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                _context.ModifierGroups.Add(modifierGroup);
                _context.SaveChanges();
                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds;
                foreach (var modifierId in modifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifierId,
                        ModifiergroupId = modifierGroup.Id
                    };
                    _context.ModifierModifiergroupMappings.Add(modifierModifierGroupMapping);
                    _context.SaveChanges();
                }
                return new JsonResult(new { success = true, message = "Modifier group added successfully" });
            } else {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                var modifierGroup = _context.ModifierGroups.FirstOrDefault(m => m.Id == createModifierGroupViewModel.ModifierGroupId);
                if (modifierGroup == null)
                {
                    return new JsonResult(new { success = false, message = "Modifier group not found" });
                }
                if (_context.ModifierGroups.Any(m => m.Name == createModifierGroupViewModel.ModifierGroupName && m.Id != createModifierGroupViewModel.ModifierGroupId))
                {
                    return new JsonResult(new { success = false, message = "Modifier group already exists" });
                }
                modifierGroup.Name = createModifierGroupViewModel.ModifierGroupName;
                modifierGroup.Description = createModifierGroupViewModel.ModifierGroupDescription;
                modifierGroup.UpdatedBy = userId;
                modifierGroup.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds;
                var existingModifierGroupIds = _context.ModifierModifiergroupMappings.Where(m => m.ModifiergroupId == modifierGroup.Id).Select(m => m.ModifierId).ToList();
                var newModifierGroupIds = modifierGroupIds.Except(existingModifierGroupIds).ToList();
                var deleteModifierGroupIds = existingModifierGroupIds.Except(modifierGroupIds).ToList();
                foreach (var modifierId in newModifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifierId,
                        ModifiergroupId = modifierGroup.Id
                    };
                    _context.ModifierModifiergroupMappings.Add(modifierModifierGroupMapping);
                    _context.SaveChanges();
                }
                foreach (var modifierId in deleteModifierGroupIds)
                {
                    var modifierModifierGroupMapping = _context.ModifierModifiergroupMappings.FirstOrDefault(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroup.Id);
                    _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                    _context.SaveChanges();
                }
                return new JsonResult(new { success = true, message = "Modifier group updated successfully" });
            }
        }

        public MenuViewModel GetModifierGroupDetails(int modifierGroupId)
        {
            var modifierGroup = _context.ModifierGroups.FirstOrDefault(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                var MenuViewModel1 = new MenuViewModel();
                return MenuViewModel1;
            }
            var selectedModifiers = _context.ModifierModifiergroupMappings.Where(m => m.ModifiergroupId == modifierGroupId).Select(m => m.Modifier).ToList();
            var modifierIds = selectedModifiers.Select(m => m.Id).ToList();
            var modifiers1 = _context.Modifiers.Where(m => m.Id == 1).ToList();
            var modifierGroups = GetModifierGroups();
            var modifiers = GetAllModifiers(null);
            CreateModifierGroupViewModel createModifierGroupViewModel = new CreateModifierGroupViewModel
            {
                ModifierGroupId = modifierGroup.Id,
                ModifierGroupName = modifierGroup.Name,
                ModifierGroupDescription = modifierGroup.Description,
                Modifiers = selectedModifiers,
                SelectedModifierIds = modifierIds
            };
            MenuViewModel menuViewModel = new MenuViewModel
            {
                CreateModifierGroupViewModel = createModifierGroupViewModel,
                AllModifiers = modifiers.Skip(0).Take(5).ToList(),
                ModifierGroups = modifierGroups,
                Modifiers = modifiers1,
                PageIndexAllModifiers = 1,
                TotalAllModifiers = modifiers.Count,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(modifiers.Count / (double)5),
            };
            return menuViewModel;
        }
    }
}