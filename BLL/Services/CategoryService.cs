using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Where(c => c.IsDeleted == false).OrderBy(c => c.Id).ToListAsync();
        }
        public async Task<List<ModifierGroup>> GetModifierGroupsAsync()
        {
            return await _context.ModifierGroups.Where(m => m.IsDeleted == false).OrderBy(m => m.Id).ToListAsync();
        }
        public async Task<ModifierGroup> GetModifierGroupAsync(int modifierGroupId)
        {
            return await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId) ?? new ModifierGroup();
        }
        public async Task<List<ModifierGroup>> GetModifierGroupsFromListAsync(List<int> modifierGroupIds)
        {
            return await _context.ModifierGroups.Where(m => modifierGroupIds.Contains(m.Id)).ToListAsync();
        }
        public async Task<List<Modifier>> GetModifiersFromListAsync(List<int> modifierGroupIds)
        {
            List<Modifier> selectedModifiers = await _context.ModifierModifiergroupMappings
            .Where(m => modifierGroupIds.Contains(m.ModifiergroupId))
            .Select(m => m.Modifier)
            .ToListAsync();

            return selectedModifiers;
        }
        public async Task<List<ModifierModifiergroupMapping>> GetModifierModifierGroupMappingsAsync(List<int> modifierGroupIds)
        {
            return await _context.ModifierModifiergroupMappings.Where(m => modifierGroupIds.Contains(m.ModifiergroupId)).ToListAsync();
        }
        public async Task<JsonResult> AddCategoryAsync(string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower() && c.IsDeleted == false))
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
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Category added successfully" });
        }
        public async Task<JsonResult> UpdateCategoryAsync(int categoryId, string categoryName, string categoryDescription, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new { success = false, message = "Category not found" });
            }
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower() && c.Id != categoryId && c.IsDeleted == false))
            {
                return new JsonResult(new { success = false, message = "Category already exists" });
            }
            category.Name = categoryName;
            category.Description = categoryDescription;
            category.UpdatedBy = userId;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Category updated successfully" });
        }
        public async Task<JsonResult> DeleteCategoryAsync(int categoryId, int userId)
        {
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                return new JsonResult(new { success = false, message = "Category not found" });
            }
            category.IsDeleted = true;
            category.UpdatedBy = userId;
            category.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            List<Item> items = await _context.Items.Where(i => i.CategoryId == categoryId).ToListAsync();
            foreach (Item item in items)
            {
                item.IsDeleted = true;
                item.UpdatedBy = userId;
                item.UpdatedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Category deleted successfully" });
        }
        public async Task<List<Item>> GetItemsBasedOnSearchAsync(int categoryId, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return await _context.Items.Where(i => i.CategoryId == categoryId && i.IsDeleted == false).OrderBy(i => i.Id).ToListAsync();
            }
            return await _context.Items.Where(i => i.CategoryId == categoryId && i.Name.ToLower().Contains(searchValue) && i.IsDeleted == false).OrderBy(i => i.Id).ToListAsync();
        }
        public async Task UpdateItemAvailabilityAsync(int itemId, bool isAvailable, int userId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                return;
            }
            item.IsAvailable = isAvailable;
            item.UpdatedBy = userId;
            await _context.SaveChangesAsync();
        }
        public async Task<IActionResult> DeleteItemAsync(int itemId, int userId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                return new JsonResult(new { success = false, message = "Item not found" });
            }
            item.IsDeleted = true;
            item.UpdatedBy = userId;
            var itemModifierGroups = await _context.ItemModifiergroups.Where(i => i.ItemId == itemId).ToListAsync();
            foreach (var itemModifierGroup in itemModifierGroups)
            {
                _context.ItemModifiergroups.Remove(itemModifierGroup);
            }
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Item deleted successfully" });
        }
        public async Task<string> AddItemAsync(AddItemViewModel addItemViewModel, int userId)
        {
            if (addItemViewModel.Id == -1)
            {
                if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.IsDeleted == false))
                {
                    return null;
                }
                else if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.IsDeleted == true))
                {
                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == addItemViewModel.ItemName && i.IsDeleted == true);
                    item.IsDeleted = false;
                    item.CategoryId = addItemViewModel.CategoryId;
                    item.Name = addItemViewModel.ItemName;
                    item.ItemType = addItemViewModel.Type;
                    item.Price = addItemViewModel.Rate;
                    item.Quantity = addItemViewModel.Quantity;
                    item.Unit = addItemViewModel.Unit;
                    item.IsAvailable = addItemViewModel.IsAvailable;
                    item.DefaultTax = addItemViewModel.IsDefaultTaxable;
                    item.TaxPercentage = addItemViewModel.TaxPercentage ?? 0;
                    item.ShortCode = addItemViewModel.ShortCode ?? "";
                    item.Description = addItemViewModel.Description;
                    item.UpdatedBy = userId;
                    item.UpdatedAt = DateTime.Now;

                    if (addItemViewModel.Image != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItemViewModel.Image.FileName);
                        if (!addItemViewModel.Image.ContentType.Contains("image"))
                        {
                            return "thisisnotacceptable";
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await addItemViewModel.Image.CopyToAsync(fileStream);
                        }
                        item.ImageUrl = fileName;
                    }

                    item.IsAvailable = addItemViewModel.Quantity > 0;
                    await _context.SaveChangesAsync();
                    return item.Name;
                }
                else
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
                        TaxPercentage = addItemViewModel.TaxPercentage ?? 0,
                        ShortCode = addItemViewModel.ShortCode ?? "",
                        Description = addItemViewModel.Description,
                        CreatedBy = userId,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        UpdatedAt = DateTime.Now,
                        UpdatedBy = userId
                    };

                    if (addItemViewModel.Image != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItemViewModel.Image.FileName);
                        if (!addItemViewModel.Image.ContentType.Contains("image"))
                        {
                            return "thisisnotacceptable";
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await addItemViewModel.Image.CopyToAsync(fileStream);
                        }
                        item.ImageUrl = fileName;
                    }

                    item.IsAvailable = addItemViewModel.Quantity > 0;
                    await _context.Items.AddAsync(item);
                    await _context.SaveChangesAsync();
                    return item.Name;
                }
            }
            else
            {
                if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.Id != addItemViewModel.Id && i.IsDeleted == false))
                {
                    return null;
                }
                else if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.Id != addItemViewModel.Id && i.IsDeleted == true))
                {
                    var itemToDelete = await _context.Items.FirstOrDefaultAsync(i => i.Id == addItemViewModel.Id && i.IsDeleted == true);
                    if (itemToDelete != null)
                    {
                        itemToDelete.IsDeleted = true;
                        itemToDelete.UpdatedBy = userId;
                        itemToDelete.UpdatedAt = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }

                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == addItemViewModel.ItemName && i.Id != addItemViewModel.Id && i.IsDeleted == true);
                    item.IsDeleted = false;
                    item.CategoryId = addItemViewModel.CategoryId;
                    item.Name = addItemViewModel.ItemName;
                    item.ItemType = addItemViewModel.Type;
                    item.Price = addItemViewModel.Rate;
                    item.Quantity = addItemViewModel.Quantity;
                    item.Unit = addItemViewModel.Unit;
                    item.IsAvailable = addItemViewModel.IsAvailable;
                    item.DefaultTax = addItemViewModel.IsDefaultTaxable;
                    item.TaxPercentage = addItemViewModel.TaxPercentage ?? 0;
                    item.ShortCode = addItemViewModel.ShortCode ?? "";
                    item.Description = addItemViewModel.Description;
                    item.UpdatedBy = userId;
                    item.UpdatedAt = DateTime.Now;

                    if (addItemViewModel.Image != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItemViewModel.Image.FileName);
                        if (!addItemViewModel.Image.ContentType.Contains("image"))
                        {
                            return "thisisnotacceptable";
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await addItemViewModel.Image.CopyToAsync(fileStream);
                        }
                        item.ImageUrl = fileName;
                    }

                    item.IsAvailable = addItemViewModel.Quantity > 0;
                    await _context.SaveChangesAsync();
                    return item.Name;
                }
                else
                {
                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == addItemViewModel.Id);
                    item.CategoryId = addItemViewModel.CategoryId;
                    item.Name = addItemViewModel.ItemName;
                    item.ItemType = addItemViewModel.Type;
                    item.Price = addItemViewModel.Rate;
                    item.Quantity = addItemViewModel.Quantity;
                    item.Unit = addItemViewModel.Unit;
                    item.IsAvailable = addItemViewModel.IsAvailable;
                    item.DefaultTax = addItemViewModel.IsDefaultTaxable;
                    item.TaxPercentage = addItemViewModel.TaxPercentage ?? 0;
                    item.ShortCode = addItemViewModel.ShortCode ?? "";
                    item.Description = addItemViewModel.Description;
                    item.UpdatedBy = userId;
                    item.UpdatedAt = DateTime.Now;
                    item.IsDeleted = false;

                    if (addItemViewModel.Image != null)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItemViewModel.Image.FileName);
                        if (!addItemViewModel.Image.ContentType.Contains("image"))
                        {
                            return "thisisnotacceptable";
                        }
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await addItemViewModel.Image.CopyToAsync(fileStream);
                        }
                        item.ImageUrl = fileName;
                    }

                    item.IsAvailable = addItemViewModel.Quantity > 0;
                    await _context.SaveChangesAsync();
                    return item.Name;
                }
            }
        }
        public async Task<IActionResult> UpdateItemModifierGroupAsync(AddItemViewModel addItemViewModel, string itemName, int userId)
        {
            if (itemName == null)
            {
                return new JsonResult(new { success = false, message = "Invalid Image" });
            }
            if (addItemViewModel.Id == -1)
            {
                if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
                {
                    return new JsonResult(new { success = true, message = "Item added successfully" });
                }
                var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds);
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
                {
                    return new JsonResult(new { success = true, message = "Item added successfully" });
                }
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == itemName);
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
                    await _context.ItemModifiergroups.AddAsync(itemModifierMapping);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true, message = "Item added successfully" });
            }
            else
            {
                if (addItemViewModel.ModifierGroupIds == null)
                {
                    var itemModifierGroups1 = await _context.ItemModifiergroups.Where(i => i.ItemId == addItemViewModel.Id).ToListAsync();
                    foreach (var itemModifierGroup in itemModifierGroups1)
                    {
                        _context.ItemModifiergroups.Remove(itemModifierGroup);
                        await _context.SaveChangesAsync();
                    }
                    return new JsonResult(new { success = true, message = "Item updated successfully" });
                }
                var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds);
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
                {
                    return new JsonResult(new { success = true, message = "Item updated successfully" });
                }
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == addItemViewModel.Id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }
                var modifierGroupIds = modifierGroupData.Select(m => m.Id).ToList();
                var itemModifierGroups = await _context.ItemModifiergroups.Where(i => i.ItemId == item.Id).ToListAsync();
                foreach (var itemModifierGroup in itemModifierGroups)
                {
                    _context.ItemModifiergroups.Remove(itemModifierGroup);
                    await _context.SaveChangesAsync();
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
                    await _context.ItemModifiergroups.AddAsync(itemModifierMapping);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true, message = "Item updated successfully" });
            }
        }
        public async Task<MenuViewModel> GetItemDataAsync(int itemId)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null)
            {
                var menuViewModel1 = new MenuViewModel();
                return menuViewModel1;
            }
            var itemModifierGroups = await _context.ItemModifiergroups.Where(i => i.ItemId == itemId).ToListAsync();
            var modifierGroupData = new List<ModifierGroupData>();
            foreach (var itemModifierGroup in itemModifierGroups)
            {
                var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == itemModifierGroup.ModifiergroupId);
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
            List<ModifierGroup> modifierGroups = await GetModifierGroupsAsync();
            List<ModifierGroup> selectedModifierGroups = await GetModifierGroupsFromListAsync(modifierGroupData.Select(x => x.Id).ToList());
            List<Modifier> selectedModifiers = await GetModifiersFromListAsync(modifierGroupData.Select(x => x.Id).ToList());
            List<Category> categories = await GetCategoriesAsync();
            List<ModifierModifiergroupMapping> selectedModifierModifierGroupMappings = await GetModifierModifierGroupMappingsAsync(modifierGroupData.Select(x => x.Id).ToList());

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
        public async Task<List<Modifier>> GetModifiersBasedOnSearchAsync(int modifierGroupId, string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return await _context.ModifierModifiergroupMappings
                    .Where(m => m.ModifiergroupId == modifierGroupId)
                    .OrderBy(m => m.ModifierId)
                    .Select(m => m.Modifier)
                    .ToListAsync();
            }
            return await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifiergroupId == modifierGroupId && m.Modifier.Name.ToLower().Contains(searchValue))
            .OrderBy(m => m.ModifierId)
            .Select(m => m.Modifier)
            .ToListAsync();
        }
        public async Task<int> GetModifierGroupIdAsync(int modifierId)
        {
            var mapping = await _context.ModifierModifiergroupMappings.FirstOrDefaultAsync(m => m.ModifierId == modifierId);
            return mapping?.ModifiergroupId ?? 0;
        }
        public async Task<IActionResult> DeleteModifierAsync(int modifierId, int userId, int modifierGroupId)
        {
            var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
            if (modifier == null)
            {
                return new JsonResult(new { success = false, message = "Modifier not found" });
            }
            var modifierModifierGroupMapping = await _context.ModifierModifiergroupMappings
            .FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroupId);
            if (modifierModifierGroupMapping != null)
            {
                _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                await _context.SaveChangesAsync();
            }
            return new JsonResult(new { success = true, message = "Modifier deleted successfully" });
        }

        public async Task<IActionResult> DeleteModifierGroupAsync(int modifierGroupId, int userId)
        {
            var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                return new JsonResult(new { success = false, message = "Modifier group not found" });
            }
            var modifierModifierGroupMappings = await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifiergroupId == modifierGroupId)
            .ToListAsync();
            foreach (var modifierModifierGroupMapping in modifierModifierGroupMappings)
            {
                _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
            }
            modifierGroup.IsDeleted = true;
            modifierGroup.UpdatedBy = userId;
            modifierGroup.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Modifier group deleted successfully" });
        }

        public async Task<List<Modifier>> GetAllModifiersAsync(string searchValue)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return await _context.Modifiers.Where(m => m.IsDeleted == false).ToListAsync();
            }
            return await _context.Modifiers
            .Where(m => m.Name.ToLower().Contains(searchValue) && m.IsDeleted == false)
            .ToListAsync();
        }

        public async Task<JsonResult> AddModifierGroupAsync(CreateModifierGroupViewModel createModifierGroupViewModel, int userId)
        {
            if (createModifierGroupViewModel.ModifierGroupId == -1)
            {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (await _context.ModifierGroups.AnyAsync(m => m.Name.ToLower() == createModifierGroupViewModel.ModifierGroupName.ToLower() && m.IsDeleted == false))
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
                await _context.ModifierGroups.AddAsync(modifierGroup);
                await _context.SaveChangesAsync();
                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds;
                foreach (var modifierId in modifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifierId,
                        ModifiergroupId = modifierGroup.Id
                    };
                    await _context.ModifierModifiergroupMappings.AddAsync(modifierModifierGroupMapping);
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Modifier group added successfully", id = modifierGroup.Id });
            }
            else
            {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == createModifierGroupViewModel.ModifierGroupId);
                if (modifierGroup == null)
                {
                    return new JsonResult(new { success = false, message = "Modifier group not found" });
                }
                if (await _context.ModifierGroups.AnyAsync(m => m.Name.ToLower() == createModifierGroupViewModel.ModifierGroupName.ToLower() && m.Id != createModifierGroupViewModel.ModifierGroupId))
                {
                    return new JsonResult(new { success = false, message = "Modifier group already exists" });
                }
                modifierGroup.Name = createModifierGroupViewModel.ModifierGroupName;
                modifierGroup.Description = createModifierGroupViewModel.ModifierGroupDescription;
                modifierGroup.UpdatedBy = userId;
                modifierGroup.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds;
                var existingModifierGroupIds = await _context.ModifierModifiergroupMappings
                    .Where(m => m.ModifiergroupId == modifierGroup.Id)
                    .Select(m => m.ModifierId)
                    .ToListAsync();
                var newModifierGroupIds = modifierGroupIds.Except(existingModifierGroupIds).ToList();
                var deleteModifierGroupIds = existingModifierGroupIds.Except(modifierGroupIds).ToList();
                foreach (var modifierId in newModifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifierId,
                        ModifiergroupId = modifierGroup.Id
                    };
                    await _context.ModifierModifiergroupMappings.AddAsync(modifierModifierGroupMapping);
                }
                foreach (var modifierId in deleteModifierGroupIds)
                {
                    var modifierModifierGroupMapping = await _context.ModifierModifiergroupMappings
                    .FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroup.Id);
                    _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Modifier group updated successfully" });
            }
        }
        public async Task<IActionResult> DeleteSelectedModifiersAsync(List<int> modifierIds, int modifierGroupId, int userId)
        {
            if (modifierIds.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No modifiers selected" });
            }
            var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                return new JsonResult(new { success = false, message = "Modifier group not found" });
            }
            foreach (var modifierId in modifierIds)
            {
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
                if (modifier == null)
                {
                    return new JsonResult(new { success = false, message = "Modifier not found" });
                }
                modifier.UpdatedBy = userId;
                modifier.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                var modifierModifierGroupMapping = await _context.ModifierModifiergroupMappings.FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroupId);
                if (modifierModifierGroupMapping != null)
                {
                    _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                    await _context.SaveChangesAsync();
                }
            }
            return new JsonResult(new { success = true, message = "Modifiers deleted successfully" });
        }

        public async Task<IActionResult> DeleteSelectedItemsAsync(List<int> itemIds, int userId)
        {
            if (itemIds.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No items selected" });
            }
            foreach (var itemId in itemIds)
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }
                item.IsDeleted = true;
                item.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                var itemModifierGroups = await _context.ItemModifiergroups.Where(i => i.ItemId == itemId).ToListAsync();
                foreach (var itemModifier in itemModifierGroups)
                {
                    _context.ItemModifiergroups.Remove(itemModifier);
                }
                await _context.SaveChangesAsync();
            }
            return new JsonResult(new { success = true, message = "Items deleted successfully" });
        }

        public async Task<MenuViewModel> GetMenuViewModelAsync(int pageIndex, int pageSize, string searchValue)
        {
            var categories = await GetCategoriesAsync();
            int categoryId = categories.FirstOrDefault()?.Id ?? 0;
            var items = await GetItemsBasedOnSearchAsync(categoryId, searchValue ?? "");
            var modifierGroups = await GetModifierGroupsAsync();
            var modifierId = modifierGroups.FirstOrDefault()?.Id ?? 0;
            var modifiers = await GetModifiersBasedOnSearchAsync(modifierId, searchValue ?? "");
            var allModifiers = await GetAllModifiersAsync(searchValue ?? "");
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
                PageIndexModifier = 1,
                PageSizeModifier = 5,
                TotalPagesModifier = (int)Math.Ceiling(modifiers.Count / 5.0),
                TotalModifiers = modifiers.Count,
                PageIndexAllModifiers = 1,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / 5.0),
                TotalAllModifiers = allModifiers.Count,
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
            return menuViewModel;
        }

        public async Task<MenuViewModel> GetMenuViewModelBasedOnFilterAsync(int categoryId, int pageIndex = 1, int pageSize = 5, string searchValue = "")
        {
            var categories = await GetCategoriesAsync();
            var items = await GetItemsBasedOnSearchAsync(categoryId, searchValue ?? "");
            int totalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
            if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            var menuViewModel = new MenuViewModel
            {
                Categories = categories,
                Items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                TotalItems = items.Count
            };
            return menuViewModel;
        }
        public async Task<MenuViewModel> GetModifierGroupDataAsync(string modifierGroupIds)
        {
            if (modifierGroupIds == "[]")
            {
                return new MenuViewModel();
            }
            var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(modifierGroupIds) ?? new List<ModifierGroupData>();
            var selectedModifierGroups = await GetModifierGroupsFromListAsync(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var selectedModifiers = await GetModifiersFromListAsync(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var selectedModifierModifierGroupMappings = await GetModifierModifierGroupMappingsAsync(modifierGroupData?.Select(x => x.Id).ToList() ?? new List<int>());
            var menuViewModel = new MenuViewModel
            {
                SelectedModifierGroups = selectedModifierGroups,
                SelectedModifiers = selectedModifiers,
                SelectedModifierModifierGroupMappings = selectedModifierModifierGroupMappings,
                ModifierGroupData = modifierGroupData ?? new List<ModifierGroupData>()
            };
            return menuViewModel;
        }

        public async Task<MenuViewModel> ModifiersFilterAsync(int pageIndex, int pageSize, int modifierGroupId, string? searchValue = null)
        {
            var modifierGroups = await GetModifierGroupsAsync();
            var modifiers = await GetModifiersBasedOnSearchAsync(modifierGroupId, searchValue ?? "");
            int totalPages = (int)Math.Ceiling(modifiers.Count / (double)pageSize);
            if (pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            var menuViewModel = new MenuViewModel
            {
                ModifierGroups = modifierGroups,
                Modifiers = modifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexModifier = pageIndex,
                PageSizeModifier = pageSize,
                TotalPagesModifier = (int)Math.Ceiling(modifiers.Count / (double)pageSize),
                TotalModifiers = modifiers.Count
            };
            return menuViewModel;
        }
        public async Task<MenuViewModel> AllModifiersFilterAsync(int pageIndex, int pageSize, string? searchValue = null)
        {
            var allModifiers = await GetAllModifiersAsync(searchValue ?? "");
            var menuViewModel = new MenuViewModel
            {
                AllModifiers = allModifiers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndexAllModifiers = pageIndex,
                PageSizeAllModifiers = pageSize,
                TotalPagesAllModifiers = (int)Math.Ceiling(allModifiers.Count / (double)pageSize),
                TotalAllModifiers = allModifiers.Count
            };
            return menuViewModel;
        }
        public async Task<MenuViewModel> RefreshItemsPartialAsync(int categoryId = -1, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            var categories = await GetCategoriesAsync();
            categoryId = categoryId == -1 ? categories.FirstOrDefault()?.Id ?? 0 : categoryId;
            var items = await GetItemsBasedOnSearchAsync(categoryId, searchValue ?? "");
            var modifierGroups = await GetModifierGroupsAsync();
            var modifierId = modifierGroups.FirstOrDefault()?.Id ?? 0;
            var modifiers = await GetModifiersBasedOnSearchAsync(modifierId, searchValue ?? "");
            var pageIndexModifier = 1;
            var pageSizeModifier = 5;
            var totalModifiers = modifiers.Count;
            var allModifiers = await GetAllModifiersAsync(searchValue ?? "");
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
            return menuViewModel;
        }
        public async Task<MenuViewModel> RefreshModifiersPartialAsync(int modifierGroupId, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            var categories = await GetCategoriesAsync();
            int categoryId = categories.FirstOrDefault()?.Id ?? 0;
            var items = await GetItemsBasedOnSearchAsync(categoryId, searchValue ?? "");
            var modifierGroups = await GetModifierGroupsAsync();
            var modifierId = modifierGroupId == -1 || modifierGroupId == 0
            ? (modifierGroups.FirstOrDefault()?.Id ?? 1)
            : modifierGroupId;
            var modifiers = await GetModifiersBasedOnSearchAsync(modifierId, searchValue ?? "");
            var pageIndexModifier = 1;
            var pageSizeModifier = 5;
            var totalModifiers = modifiers.Count;
            var allModifiers = await GetAllModifiersAsync(searchValue ?? "");
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
            return menuViewModel;
        }
        public async Task<MenuViewModel> ResetAddItemForm()
        {
            var categories = await GetCategoriesAsync();
            var modifierGroups = await GetModifierGroupsAsync();
            MenuViewModel menuViewModel = new MenuViewModel
            {
                Categories = categories,
                ModifierGroups = modifierGroups

            };
            return menuViewModel;
        }
        public async Task<MenuViewModel> GetModifierGroupDetailsAsync(int modifierGroupId)
        {
            var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                return new MenuViewModel();
            }

            var selectedModifiers = await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifiergroupId == modifierGroupId)
            .Select(m => m.Modifier)
            .ToListAsync();

            var modifierIds = selectedModifiers.Select(m => m.Id).ToList();
            var modifiers1 = await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifiergroupId == modifierGroupId)
            .Select(m => m.Modifier)
            .ToListAsync();

            var modifierGroups = await GetModifierGroupsAsync();
            var modifiers = await GetAllModifiersAsync(null);

            var createModifierGroupViewModel = new CreateModifierGroupViewModel
            {
                ModifierGroupId = modifierGroup.Id,
                ModifierGroupName = modifierGroup.Name,
                ModifierGroupDescription = modifierGroup.Description,
                Modifiers = selectedModifiers,
                SelectedModifierIds = modifierIds
            };

            var menuViewModel = new MenuViewModel
            {
                CreateModifierGroupViewModel = createModifierGroupViewModel,
                AllModifiers = modifiers.Skip(0).Take(5).ToList(),
                ModifierGroups = modifierGroups,
                Modifiers = modifiers1.Skip(0).Take(5).ToList(),
                PageIndexAllModifiers = 1,
                TotalAllModifiers = modifiers.Count,
                PageSizeAllModifiers = 5,
                TotalPagesAllModifiers = (int)Math.Ceiling(modifiers.Count / (double)5),
                PageIndexModifier = 1,
                TotalModifiers = modifiers1.Count,
                PageSizeModifier = 5,
                TotalPagesModifier = (int)Math.Ceiling(modifiers1.Count / (double)5)
            };

            return menuViewModel;
        }
        public async Task<MenuViewModel> GetModifierDataAsync(int modifierId, int modifierGroupId)
        {
            var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
            if (modifier == null)
            {
                return new MenuViewModel();
            }

            var modifiers1 = await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifiergroupId == modifierGroupId)
            .OrderBy(m => m.ModifierId)
            .Select(m => m.Modifier)
            .ToListAsync();

            var selectedModifierGroups = await _context.ModifierModifiergroupMappings
            .Where(m => m.ModifierId == modifierId)
            .Select(m => m.Modifiergroup)
            .ToListAsync();

            var modifierGroupIds = selectedModifierGroups.Select(m => m.Id).ToList();
            var modifierGroups = await GetModifierGroupsAsync();
            var categories = await GetCategoriesAsync();
            var allModifiers = await GetAllModifiersAsync(null);

            var addModifierViewModel = new AddModifierViewModel
            {
                Id = modifier.Id,
                Name = modifier.Name,
                Rate = modifier.Price,
                Quantity = (int)modifier.Quantity,
                Unit = modifier.Unit,
                Description = modifier.Description,
                ModifierGroupIds = modifierGroupIds
            };

            var menuViewModel = new MenuViewModel
            {
                ModifierGroups = modifierGroups,
                AddModifierViewModel = addModifierViewModel,
                Categories = categories,
                AllModifiers = allModifiers,
                Modifiers = modifiers1.Skip(0).Take(5).ToList(),
                PageIndexModifier = 1,
                TotalModifiers = modifiers1.Count,
                PageSizeModifier = 5,
                TotalPagesModifier = (int)Math.Ceiling(modifiers1.Count / (double)5),
            };

            return menuViewModel;
        }
        public async Task<MenuViewModel> ResetAddItemFormAsync()
        {
            List<Category> categories = await GetCategoriesAsync();
            List<ModifierGroup> modifierGroups = await GetModifierGroupsAsync();
            MenuViewModel menuViewModel = new MenuViewModel
            {
                Categories = categories,
                ModifierGroups = modifierGroups

            };
            return menuViewModel;
        }
        public async Task<IActionResult> AddModifierAsync(AddModifierViewModel addModifierViewModel, int userId)
        {
            if (addModifierViewModel.Id == -1)
            {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                if (await _context.Modifiers.AnyAsync(m => m.Name.ToLower() == addModifierViewModel.Name.ToLower()))
                {
                    return new JsonResult(new { success = false, message = "Modifier already exists" });
                }
                var modifier = new Modifier
                {
                    Name = addModifierViewModel.Name,
                    Description = addModifierViewModel.Description,
                    Price = addModifierViewModel.Rate,
                    Quantity = addModifierViewModel.Quantity,
                    Unit = addModifierViewModel.Unit,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _context.Modifiers.AddAsync(modifier);
                await _context.SaveChangesAsync();
                var modifierGroupIds = addModifierViewModel.ModifierGroupIds;
                foreach (var modifierGroupId in modifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifier.Id,
                        ModifiergroupId = modifierGroupId
                    };
                    await _context.ModifierModifiergroupMappings.AddAsync(modifierModifierGroupMapping);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true, message = "Modifier added successfully" });
            }
            else
            {
                if (userId == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == addModifierViewModel.Id);
                if (modifier == null)
                {
                    return new JsonResult(new { success = false, message = "Modifier not found" });
                }
                if (await _context.Modifiers.AnyAsync(m => m.Name.ToLower() == addModifierViewModel.Name.ToLower() && m.Id != addModifierViewModel.Id))
                {
                    return new JsonResult(new { success = false, message = "Modifier already exists" });
                }
                modifier.Name = addModifierViewModel.Name;
                modifier.Description = addModifierViewModel.Description;
                modifier.Price = addModifierViewModel.Rate;
                modifier.Unit = addModifierViewModel.Unit;
                modifier.Quantity = addModifierViewModel.Quantity;
                modifier.UpdatedBy = userId;
                modifier.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                var modifierGroupIds = addModifierViewModel.ModifierGroupIds;
                var existingModifierGroupIds = await _context.ModifierModifiergroupMappings
                    .Where(m => m.ModifierId == modifier.Id)
                    .Select(m => m.ModifiergroupId)
                    .ToListAsync();
                var newModifierGroupIds = modifierGroupIds.Except(existingModifierGroupIds).ToList();
                var deleteModifierGroupIds = existingModifierGroupIds.Except(modifierGroupIds).ToList();
                foreach (var modifierGroupId in newModifierGroupIds)
                {
                    var modifierModifierGroupMapping = new ModifierModifiergroupMapping
                    {
                        ModifierId = modifier.Id,
                        ModifiergroupId = modifierGroupId
                    };
                    await _context.ModifierModifiergroupMappings.AddAsync(modifierModifierGroupMapping);
                    await _context.SaveChangesAsync();
                }
                foreach (var modifierGroupId in deleteModifierGroupIds)
                {
                    var modifierModifierGroupMapping = await _context.ModifierModifiergroupMappings
                    .FirstOrDefaultAsync(m => m.ModifierId == modifier.Id && m.ModifiergroupId == modifierGroupId);
                    _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                    await _context.SaveChangesAsync();
                }
                return new JsonResult(new { success = true, message = "Modifier updated successfully" });
            }
        }
    }
}