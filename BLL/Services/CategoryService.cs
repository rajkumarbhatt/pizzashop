using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<CategoryService> _logger;
        public CategoryService(PizzaShopContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Categories.Where(c => c.IsDeleted == false).OrderBy(c => c.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching categories.");
                return new List<Category>();
            }
        }
        public async Task<List<ModifierGroup>> GetModifierGroupsAsync()
        {
            try
            {
                return await _context.ModifierGroups.Where(m => m.IsDeleted == false).OrderBy(m => m.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching modifier groups.");
                return new List<ModifierGroup>();
            }
        }
        public async Task<ModifierGroup> GetModifierGroupAsync(int modifierGroupId)
        {
            try
            {
                return await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId) ?? new ModifierGroup();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the modifier group with ID {modifierGroupId}.");
                return new ModifierGroup();
            }
        }
        public async Task<List<ModifierGroup>> GetModifierGroupsFromListAsync(List<int> modifierGroupIds)
        {
            try
            {
                return await _context.ModifierGroups.Where(m => modifierGroupIds.Contains(m.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching modifier groups from the list.");
                return new List<ModifierGroup>();
            }
        }
        public async Task<List<Modifier>> GetModifiersFromListAsync(List<int> modifierGroupIds)
        {
            try
            {
                List<Modifier> selectedModifiers = await _context.ModifierModifiergroupMappings
                    .Where(m => modifierGroupIds.Contains(m.ModifiergroupId))
                    .Select(m => m.Modifier)
                    .ToListAsync();

                return selectedModifiers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching modifiers from the list.");
                return new List<Modifier>();
            }
        }
        public async Task<List<ModifierModifiergroupMapping>> GetModifierModifierGroupMappingsAsync(List<int> modifierGroupIds)
        {
            try
            {
                return await _context.ModifierModifiergroupMappings.Where(m => modifierGroupIds.Contains(m.ModifiergroupId)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching modifier group mappings.");
                return new List<ModifierModifiergroupMapping>();
            }
        }
        public async Task<JsonResult> AddCategoryAsync(AddEditCategoryViewModel addEditCategoryViewModel, int userId)
        {
            try
            {
                string categoryName = addEditCategoryViewModel.Name ?? string.Empty;
                string categoryDescription = addEditCategoryViewModel.Description ?? string.Empty;
                int categoryId = addEditCategoryViewModel.Id;

                if (categoryId != 0)
                {
                    if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower() && c.Id != categoryId && c.IsDeleted == false))
                    {
                        _logger.LogWarning("Category with the same name already exists.");
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Category already exists"
                        });
                    }

                    Category categoryToUpdate = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId) ?? new Category();
                    if (categoryToUpdate != null)
                    {
                        categoryToUpdate.Name = categoryName;
                        categoryToUpdate.Description = categoryDescription;
                        categoryToUpdate.UpdatedBy = userId;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Category updated successfully. Category ID: {CategoryId}", categoryId);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Category updated successfully"
                        });
                    }
                }

                if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower() && c.IsDeleted == false))
                {
                    _logger.LogWarning("Category with the same name already exists.");
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Category already exists"
                    });
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
                _logger.LogInformation("Category added successfully. Category Name: {CategoryName}", categoryName);
                return new JsonResult(new
                {
                    success = true,
                    message = "Category added successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding the category"
                });
            }
        }
        public async Task<MenuViewModel> GetCategoryDetailsAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category == null)
                {
                    return new MenuViewModel();
                }
                var addEditCategoryViewModel = new AddEditCategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                };
                var menuViewModel = new MenuViewModel
                {
                    AddEditCategoryViewModal = addEditCategoryViewModel
                };
                return menuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching category details.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<JsonResult> DeleteCategoryAsync(int categoryId, int userId)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found.", categoryId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Category not found"
                    });
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

                _logger.LogInformation("Category with ID {CategoryId} and its items were deleted successfully.", categoryId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Category deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the category with ID {CategoryId}.", categoryId);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the category"
                });
            }
        }
        public async Task<List<Item>> GetItemsBasedOnSearchAsync(int categoryId, string searchValue)
        {
            try
            {
                if (string.IsNullOrEmpty(searchValue))
                {
                    return await _context.Items.Where(i => i.CategoryId == categoryId && i.IsDeleted == false).OrderBy(i => i.Id).ToListAsync();
                }
                return await _context.Items.Where(i => i.CategoryId == categoryId && i.Name.ToLower().Contains(searchValue) && i.IsDeleted == false).OrderBy(i => i.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching items based on search.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<Item>();
            }
        }
        public async Task<IActionResult> DeleteItemAsync(int itemId, int userId)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
                if (item == null)
                {
                    _logger.LogWarning("Item with ID {ItemId} not found.", itemId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Item not found"
                    });
                }
                item.IsDeleted = true;
                item.UpdatedBy = userId;
                var itemModifierGroups = await _context.ItemModifiergroups.Where(i => i.ItemId == itemId).ToListAsync();
                foreach (var itemModifierGroup in itemModifierGroups)
                {
                    _context.ItemModifiergroups.Remove(itemModifierGroup);
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Item with ID {ItemId} deleted successfully.", itemId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Item deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the item with ID {ItemId}.", itemId);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the item"
                });
            }
        }
        public async Task<string> AddItemAsync(AddItemViewModel addItemViewModel, int userId)
        {
            try
            {
                if (addItemViewModel.Id == -1)
                {
                    if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.IsDeleted == false))
                    {
                        _logger.LogWarning("Item with the same name already exists.");
                        return "itemalreadyexists";
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
                                _logger.LogWarning("Invalid image format.");
                                return "thisisnotacceptable";
                            }
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await addItemViewModel.Image.CopyToAsync(fileStream);
                            }
                            item.ImageUrl = fileName;
                        }
                        await _context.Items.AddAsync(item);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Item added successfully. Item Name: {ItemName}", item.Name);
                        return item.Name;
                    }
                }
                else
                {
                    if (await _context.Items.AnyAsync(i => i.Name.ToLower() == addItemViewModel.ItemName.ToLower() && i.Id != addItemViewModel.Id && i.IsDeleted == false))
                    {
                        _logger.LogWarning("Item with the same name already exists.");
                        return "itemalreadyexists";
                    }
                    else
                    {
                        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == addItemViewModel.Id) ?? new Item();
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
                                _logger.LogWarning("Invalid image format.");
                                return "thisisnotacceptable";
                            }
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/item-images", fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await addItemViewModel.Image.CopyToAsync(fileStream);
                            }
                            item.ImageUrl = fileName;
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Item updated successfully. Item Name: {ItemName}", item.Name);
                        return item.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding or updating the item.");
                return "error";
            }
        }
        public async Task<IActionResult> UpdateItemModifierGroupAsync(AddItemViewModel addItemViewModel, string itemName, int userId)
        {
            try
            {
                if (itemName == null)
                {
                    _logger.LogWarning("Invalid image provided.");
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Invalid Image"
                    });
                }
                if (addItemViewModel.Id == -1)
                {
                    if (string.IsNullOrEmpty(addItemViewModel.ModifierGroupIds))
                    {
                        _logger.LogInformation("Item added successfully without modifier groups.");
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Item added successfully"
                        });
                    }
                    var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds) ?? new List<ModifierGroupData>();
                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == itemName);
                    if (item == null)
                    {
                        _logger.LogWarning("Item not found. Item Name: {ItemName}", itemName);
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Item not found"
                        });
                    }
                    var modifierGroupIds = modifierGroupData.Select(m => m.Id).ToList();
                    foreach (var modifierGroupId in modifierGroupIds)
                    {
                        var itemModifierMapping = new ItemModifiergroup
                        {
                            ItemId = item.Id,
                            ModifiergroupId = modifierGroupId,
                            MinValue = (short?)modifierGroupData?.FirstOrDefault(m => m.Id == modifierGroupId)?.MinimumQuantity,
                            MaxValue = (short?)modifierGroupData?.FirstOrDefault(m => m.Id == modifierGroupId)?.MaximumQuantity,
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _context.ItemModifiergroups.AddAsync(itemModifierMapping);
                        await _context.SaveChangesAsync();
                    }
                    _logger.LogInformation("Item added successfully with modifier groups. Item Name: {ItemName}", itemName);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Item added successfully"
                    });
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
                        _logger.LogInformation("Item updated successfully without modifier groups. Item ID: {ItemId}", addItemViewModel.Id);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Item updated successfully"
                        });
                    }
                    var modifierGroupData = JsonConvert.DeserializeObject<List<ModifierGroupData>>(addItemViewModel.ModifierGroupIds) ?? new List<ModifierGroupData>();
                    var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == addItemViewModel.Id);
                    if (item == null)
                    {
                        _logger.LogWarning("Item not found. Item ID: {ItemId}", addItemViewModel.Id);
                        return new JsonResult(new
                        {
                            success = false,
                            message = "Item not found"
                        });
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
                            MinValue = (short?)modifierGroupData?.FirstOrDefault(m => m.Id == modifierGroupId)?.MinimumQuantity,
                            MaxValue = (short?)modifierGroupData?.FirstOrDefault(m => m.Id == modifierGroupId)?.MaximumQuantity,
                            CreatedBy = userId,
                            UpdatedBy = userId
                        };
                        await _context.ItemModifiergroups.AddAsync(itemModifierMapping);
                        await _context.SaveChangesAsync();
                    }
                    _logger.LogInformation("Item updated successfully with modifier groups. Item ID: {ItemId}", addItemViewModel.Id);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Item updated successfully"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the item modifier group.");
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating the item modifier group"
                });
            }
        }
        public async Task<MenuViewModel> GetItemDataAsync(int itemId)
        {
            try
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
                    var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == itemModifierGroup.ModifiergroupId) ?? new ModifierGroup();
                    modifierGroupData.Add(new ModifierGroupData
                    {
                        Id = modifierGroup.Id,
                        Name = modifierGroup.Name,
                        MinimumQuantity = (int)(itemModifierGroup.MinValue ?? 0),
                        MaximumQuantity = (int)(itemModifierGroup.MaxValue ?? 0),
                    });
                }
                var addItemViewModel = new AddItemViewModel
                {
                    Id = item.Id,
                    CategoryId = item.CategoryId,
                    ItemName = item.Name,
                    Type = item.ItemType,
                    Rate = item.Price,
                    Quantity = (int)(item.Quantity ?? 0),
                    Unit = item.Unit ?? "",
                    IsAvailable = (bool)(item.IsAvailable ?? false),
                    IsDefaultTaxable = (bool)(item.DefaultTax ?? false),
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching item data.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<List<Modifier>> GetModifiersBasedOnSearchAsync(int modifierGroupId, string searchValue)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching modifiers based on search.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<Modifier>();
            }
        }
        public async Task<int> GetModifierGroupIdAsync(int modifierId)
        {
            try
            {
                ModifierModifiergroupMapping mapping = await _context.ModifierModifiergroupMappings.FirstOrDefaultAsync(m => m.ModifierId == modifierId)
                    ?? new ModifierModifiergroupMapping();
                return mapping?.ModifiergroupId ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the modifier group ID.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 0;
            }
        }
        public async Task<IActionResult> DeleteModifierAsync(int modifierId, int userId, int modifierGroupId)
        {
            try
            {
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
                if (modifier == null)
                {
                    _logger.LogWarning("Modifier with ID {ModifierId} not found.", modifierId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Modifier not found"
                    });
                }
                var modifierModifierGroupMapping = await _context.ModifierModifiergroupMappings
                    .FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroupId);
                if (modifierModifierGroupMapping != null)
                {
                    _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("Modifier with ID {ModifierId} deleted successfully.", modifierId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Modifier deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the modifier with ID {ModifierId}.", modifierId);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the modifier"
                });
            }
        }
        public async Task<IActionResult> DeleteModifierGroupAsync(int modifierGroupId, int userId)
        {
            try
            {
                var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId);
                if (modifierGroup == null)
                {
                    _logger.LogWarning("Modifier group with ID {ModifierGroupId} not found.", modifierGroupId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Modifier group not found"
                    });
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
                _logger.LogInformation("Modifier group with ID {ModifierGroupId} deleted successfully.", modifierGroupId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Modifier group deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the modifier group with ID {ModifierGroupId}.", modifierGroupId);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while deleting the modifier group"
                });
            }
        }
        public async Task<List<Modifier>> GetAllModifiersAsync(string searchValue)
        {
            try
            {
                if (string.IsNullOrEmpty(searchValue))
                {
                    return await _context.Modifiers.Where(m => m.IsDeleted == false).ToListAsync();
                }
                return await _context.Modifiers
                    .Where(m => m.Name.ToLower().Contains(searchValue) && m.IsDeleted == false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all modifiers.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<Modifier>();
            }
        }
        public async Task<JsonResult> AddModifierGroupAsync(CreateModifierGroupViewModel createModifierGroupViewModel, int userId)
        {
            try
            {
            if (createModifierGroupViewModel.ModifierGroupId == -1)
            {
                if (await _context.ModifierGroups.AnyAsync(m => m.Name.ToLower() == createModifierGroupViewModel.ModifierGroupName.ToLower() && m.IsDeleted == false))
                {
                _logger.LogWarning("Modifier group with the same name already exists.");
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier group already exists"
                });
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
                _logger.LogInformation("Modifier group added successfully. Modifier Group ID: {ModifierGroupId}", modifierGroup.Id);

                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds ?? new List<int>();
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
                return new JsonResult(new
                {
                success = true,
                message = "Modifier group added successfully",
                id = modifierGroup.Id
                });
            }
            else
            {
                var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == createModifierGroupViewModel.ModifierGroupId);
                if (modifierGroup == null)
                {
                _logger.LogWarning("Modifier group with ID {ModifierGroupId} not found.", createModifierGroupViewModel.ModifierGroupId);
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier group not found"
                });
                }
                if (await _context.ModifierGroups.AnyAsync(m => m.Name.ToLower() == createModifierGroupViewModel.ModifierGroupName.ToLower() && m.Id != createModifierGroupViewModel.ModifierGroupId && m.IsDeleted == false))
                {
                _logger.LogWarning("Modifier group with the same name already exists.");
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier group already exists"
                });
                }
                modifierGroup.Name = createModifierGroupViewModel.ModifierGroupName;
                modifierGroup.Description = createModifierGroupViewModel.ModifierGroupDescription;
                modifierGroup.UpdatedBy = userId;
                modifierGroup.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Modifier group updated successfully. Modifier Group ID: {ModifierGroupId}", modifierGroup.Id);

                var modifierGroupIds = createModifierGroupViewModel.SelectedModifierIds ?? new List<int>();
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
                    .FirstOrDefaultAsync(m => m.ModifierId == modifierId && m.ModifiergroupId == modifierGroup.Id) ?? new ModifierModifiergroupMapping();
                _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new
                {
                success = true,
                message = "Modifier group updated successfully"
                });
            }
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "An error occurred while adding or updating the modifier group.");
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while adding or updating the modifier group"
            });
            }
        }
        public async Task<IActionResult> DeleteSelectedModifiersAsync(List<int> modifierIds, int modifierGroupId, int userId)
        {
            try
            {
            if (modifierIds.Count == 0)
            {
                _logger.LogWarning("No modifiers selected for deletion.");
                return new JsonResult(new
                {
                success = false,
                message = "No modifiers selected"
                });
            }
            var modifierGroup = await _context.ModifierGroups.FirstOrDefaultAsync(m => m.Id == modifierGroupId);
            if (modifierGroup == null)
            {
                _logger.LogWarning("Modifier group with ID {ModifierGroupId} not found.", modifierGroupId);
                return new JsonResult(new
                {
                success = false,
                message = "Modifier group not found"
                });
            }
            foreach (var modifierId in modifierIds)
            {
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
                if (modifier == null)
                {
                _logger.LogWarning("Modifier with ID {ModifierId} not found.", modifierId);
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier not found"
                });
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
            _logger.LogInformation("Modifiers deleted successfully from modifier group ID {ModifierGroupId}.", modifierGroupId);
            return new JsonResult(new
            {
                success = true,
                message = "Modifiers deleted successfully"
            });
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "An error occurred while deleting the selected modifiers.");
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while deleting the selected modifiers"
            });
            }
        }
        public async Task<IActionResult> DeleteSelectedItemsAsync(List<int> itemIds, int userId)
        {
            try
            {
            if (itemIds.Count == 0)
            {
                _logger.LogWarning("No items selected for deletion.");
                return new JsonResult(new
                {
                success = false,
                message = "No items selected"
                });
            }
            foreach (var itemId in itemIds)
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId);
                if (item == null)
                {
                _logger.LogWarning("Item with ID {ItemId} not found.", itemId);
                return new JsonResult(new
                {
                    success = false,
                    message = "Item not found"
                });
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
            _logger.LogInformation("Items deleted successfully.");
            return new JsonResult(new
            {
                success = true,
                message = "Items deleted successfully"
            });
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "An error occurred while deleting the selected items.");
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while deleting the selected items"
            });
            }
        }
        public async Task<MenuViewModel> GetMenuViewModelAsync(int pageIndex, int pageSize, string searchValue)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the menu view model.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> GetMenuViewModelBasedOnFilterAsync(int categoryId, int pageIndex = 1, int pageSize = 5, string searchValue = "")
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the menu view model based on filter.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> GetModifierGroupDataAsync(string modifierGroupIds)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the modifier group data.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> ModifiersFilterAsync(int pageIndex, int pageSize, int modifierGroupId, string? searchValue = null)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the modifiers filter.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> AllModifiersFilterAsync(int pageIndex, int pageSize, string? searchValue = null)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the all modifiers filter.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> RefreshItemsPartialAsync(int categoryId = -1, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            try
            {
                categoryId = categoryId == 0 ? -1 : categoryId;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing items partial.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> RefreshModifiersPartialAsync(int modifierGroupId, int pageIndex = 1, int pageSize = 5, string? searchValue = null)
        {
            try
            {
                var categories = await GetCategoriesAsync();
                int categoryId = categories.FirstOrDefault()?.Id ?? 0;
                var items = await GetItemsBasedOnSearchAsync(categoryId, searchValue ?? "");
                var modifierGroups = await GetModifierGroupsAsync();
                var modifierId = modifierGroupId == -1 || modifierGroupId == 0 ?
                    (modifierGroups.FirstOrDefault()?.Id ?? 1) :
                    modifierGroupId;
                var modifiers = await GetModifiersBasedOnSearchAsync(modifierId, searchValue ?? "");
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
                    PageIndexModifier = pageIndex,
                    PageSizeModifier = pageSize,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while refreshing modifiers partial.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> ResetAddItemForm()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the add item form.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> GetModifierGroupDetailsAsync(int modifierGroupId, int pageIndex = 1, int pageSize = 5)
        {
            try
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
                    .Select(m => m.Modifier).OrderBy(m => m.Id)
                    .ToListAsync();

                var modifierGroups = await GetModifierGroupsAsync();
                var modifiers = await GetAllModifiersAsync("");
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
                    Modifiers = modifiers1.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    PageIndexAllModifiers = 1,
                    TotalAllModifiers = modifiers.Count,
                    PageSizeAllModifiers = 5,
                    TotalPagesAllModifiers = (int)Math.Ceiling(modifiers.Count / (double)5),
                    PageIndexModifier = pageIndex,
                    TotalModifiers = modifiers1.Count,
                    PageSizeModifier = pageSize,
                    TotalPagesModifier = (int)Math.Ceiling(modifiers1.Count / (double)5)
                };

                return menuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the modifier group details.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> GetModifierDataAsync(int modifierId, int modifierGroupId)
        {
            try
            {
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId);
                if (modifier == null)
                {
                    return new MenuViewModel();
                }

                var selectedModifierGroups = await _context.ModifierModifiergroupMappings
                    .Where(m => m.ModifierId == modifierId)
                    .Select(m => m.Modifiergroup)
                    .ToListAsync();

                var modifierGroupIds = selectedModifierGroups.Select(m => m.Id).ToList();
                var modifierGroups = await GetModifierGroupsAsync();

                var addModifierViewModel = new AddModifierViewModel
                {
                    Id = modifier.Id,
                    Name = modifier.Name,
                    Rate = modifier.Price,
                    Quantity = (int)(modifier.Quantity ?? 0),
                    Unit = modifier.Unit ?? "",
                    Description = modifier.Description,
                    ModifierGroupIds = modifierGroupIds
                };

                var menuViewModel = new MenuViewModel
                {
                    ModifierGroups = modifierGroups,
                    AddModifierViewModel = addModifierViewModel,
                };

                return menuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the modifier data.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<MenuViewModel> ResetAddItemFormAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the add item form.");
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new MenuViewModel();
            }
        }
        public async Task<IActionResult> AddModifierAsync(AddModifierViewModel addModifierViewModel, int userId)
        {
            try
            {
            if (addModifierViewModel.Id == -1)
            {
                if (await _context.Modifiers.AnyAsync(m => m.Name.ToLower() == addModifierViewModel.Name.ToLower()))
                {
                _logger.LogWarning("Modifier with the same name already exists.");
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier already exists"
                });
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
                _logger.LogInformation("Modifier added successfully. Modifier ID: {ModifierId}", modifier.Id);

                var modifierGroupIds = addModifierViewModel.ModifierGroupIds ?? new List<int>();
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
                return new JsonResult(new
                {
                success = true,
                message = "Modifier added successfully"
                });
            }
            else
            {
                var modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == addModifierViewModel.Id);
                if (modifier == null)
                {
                _logger.LogWarning("Modifier with ID {ModifierId} not found.", addModifierViewModel.Id);
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier not found"
                });
                }
                if (await _context.Modifiers.AnyAsync(m => m.Name.ToLower() == addModifierViewModel.Name.ToLower() && m.Id != addModifierViewModel.Id))
                {
                _logger.LogWarning("Modifier with the same name already exists.");
                return new JsonResult(new
                {
                    success = false,
                    message = "Modifier already exists"
                });
                }
                modifier.Name = addModifierViewModel.Name;
                modifier.Description = addModifierViewModel.Description;
                modifier.Price = addModifierViewModel.Rate;
                modifier.Unit = addModifierViewModel.Unit;
                modifier.Quantity = addModifierViewModel.Quantity;
                modifier.UpdatedBy = userId;
                modifier.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Modifier updated successfully. Modifier ID: {ModifierId}", modifier.Id);

                var modifierGroupIds = addModifierViewModel.ModifierGroupIds ?? new List<int>();
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
                    .FirstOrDefaultAsync(m => m.ModifierId == modifier.Id && m.ModifiergroupId == modifierGroupId) ?? new ModifierModifiergroupMapping();
                _context.ModifierModifiergroupMappings.Remove(modifierModifierGroupMapping);
                await _context.SaveChangesAsync();
                }
                return new JsonResult(new
                {
                success = true,
                message = "Modifier updated successfully"
                });
            }
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "An error occurred while adding or updating the modifier.");
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while adding or updating the modifier"
            });
            }
        }
    }
}