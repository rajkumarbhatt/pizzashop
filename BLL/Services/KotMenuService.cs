using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class KotMenuService : IKotMenuService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<KotMenuService> _logger;
        public KotMenuService(PizzaShopContext context, ILogger<KotMenuService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<string> GetOrderStatusAsync(int orderId)
        {
            try
            {
                string orderStatus = await _context.Orders.Where(o => o.Id == orderId).Select(o => o.Status).FirstOrDefaultAsync() ?? "Pending";
                return orderStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching order status for order ID {OrderId}", orderId);
                Console.WriteLine(ex.Message);
                return "Error";
            }
        }
        public async Task<KotMenuViewModel> GetKotMenuAsync(int? orderId)
        {
            try
            {
                bool areItemsAdded = false;
                OrderDetailsCard orderDetailsCard = new OrderDetailsCard();
                List<Category> categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
                List<MenuItemsKot> menuItemsKot = await _context.Items.Where(m => m.IsDeleted == false && m.IsAvailable == true).Select(m => new MenuItemsKot
                {
                    Id = m.Id,
                    Name = m.Name,
                    Price = m.Price,
                    CategoryId = m.CategoryId,
                    Image = m.ImageUrl,
                    ItemType = m.ItemType,
                    IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
                }).ToListAsync();

                if (orderId == 0)
                {
                    KotMenuViewModel kotMenuViewModel1 = new KotMenuViewModel
                    {
                        Categories = categories,
                        MenuItemsKot = menuItemsKot
                    };
                    return kotMenuViewModel1;
                }

                if (orderId != null)
                {
                    List<int> tableIds = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).Select(otb => otb.TableId).ToListAsync();
                    int sectionId = (await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableIds[0])).SectionId;
                    string sectionName = (await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId)).Name;
                    string tableNames = "";

                    foreach (int tableId in tableIds)
                    {
                        string tableName = (await _context.Tables.FirstOrDefaultAsync(t => t.Id == tableId)).Name;
                        tableNames = string.IsNullOrEmpty(tableNames) ? tableName : $"{tableNames}, {tableName}";

                        List<OrderItemDetials> orderItemDetails = await _context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).Select(oi => new OrderItemDetials
                        {
                            OrderItemId = oi.Id,
                            ItemId = oi.ItemId,
                            ItemName = oi.Item.Name,
                            ItemQuantity = oi.Quantity,
                            ItemTotalPrice = (decimal?)(oi.Price * oi.Quantity) ?? 0,
                            Modifiers = _context.OrderModifiers.Where(om => om.OrderItemId == oi.Id && om.IsDeleted == false).Select(om => new ModifierDetails
                            {
                                ModifierId = om.ModifierId,
                                ModifierName = om.Modifier.Name,
                                ModifierPrice = (decimal?)om.Price * om.Quantity ?? 0,
                            }).ToList(),
                            ModifiersTotalPrice = (decimal?)_context.OrderModifiers.Where(om => om.OrderItemId == oi.Id && om.IsDeleted == false).Sum(om => om.Price * om.Quantity) ?? 0,
                        }).ToListAsync();
                        orderDetailsCard.OrderStatus = await GetOrderStatusAsync(orderId.Value);
                        orderDetailsCard.SectionName = sectionName;
                        orderDetailsCard.TableNames = tableNames;
                        orderDetailsCard.OrderItemDetails = orderItemDetails;
                        orderDetailsCard.SubTotal = (decimal?)_context.Orders.Where(o => o.Id == orderId).Select(o => o.SubTotal).FirstOrDefault() ?? 0;
                        orderDetailsCard.Taxes = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).Select(ot => new InvoiceTax
                        {
                            TaxName = ot.TaxId == 0 ?
                                "Other Tax" :
                                _context.TaxesFees.Any(tf => tf.Id == ot.TaxId) ?
                                _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).Name :
                                null,
                            TaxAmount = Math.Round((double)ot.TaxAmount, 2),
                            TaxType = ot.TaxId == 0 ?
                                "Fixed" :
                                _context.TaxesFees.Any(tf => tf.Id == ot.TaxId) ?
                                _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).TaxType :
                                null,
                            TaxRate = ot.TaxId == 0 ?
                                0 :
                                _context.TaxesFees.Any(tf => tf.Id == ot.TaxId) ?
                                (double?)_context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).Amount :
                                0
                        }).ToListAsync();
                        orderDetailsCard.TotalPrice = (decimal?)_context.Orders.Where(o => o.Id == orderId).Select(o => o.TotalAmount).FirstOrDefault() ?? 0;
                    }
                }
                if (orderDetailsCard.Taxes == null || orderDetailsCard.Taxes.Count == 0)
                {
                    orderDetailsCard.Taxes = await _context.TaxesFees.Where(tf => tf.IsDeleted == false && tf.IsEnabled == true).Select(tf => new InvoiceTax
                    {
                        TaxName = tf.Name,
                        TaxAmount = Math.Round((double)tf.Amount, 2),
                        TaxType = tf.TaxType,
                        TaxRate = _context.TaxesFees.Any(t => t.Id == tf.Id) ? (double?)_context.TaxesFees.FirstOrDefault(t => t.Id == tf.Id).Amount : 0
                    }).ToListAsync();
                    orderDetailsCard.Taxes.Add(new InvoiceTax
                    {
                        TaxName = "Other Tax",
                        TaxAmount = 0,
                        TaxType = "Fixed",
                        TaxRate = 0
                    });
                }
                else
                {
                    areItemsAdded = true;
                }
                List<ItemTaxes> itemTaxes = new List<ItemTaxes>();
                List<Item> itemList = await _context.Items.Where(i => i.IsDeleted == false && i.IsAvailable == true).ToListAsync();
                foreach (var item in itemList)
                {
                    ItemTaxes itemTaxes1 = new ItemTaxes
                    {
                        ItemId = item.Id,
                        IsDefault = item.DefaultTax ?? false,
                        TaxPercentage = (decimal)item.TaxPercentage,
                        ItemPrice = (decimal)item.Price
                    };
                    itemTaxes.Add(itemTaxes1);
                }
                KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
                {
                    Categories = categories,
                    MenuItemsKot = menuItemsKot,
                    OrderDetailsCard = orderDetailsCard,
                    TaxesFees = orderDetailsCard.Taxes,
                    AreItemsAdded = areItemsAdded,
                    ItemTaxes = itemTaxes,
                };

                return kotMenuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching KOT menu data");
                Console.WriteLine(ex.Message);
                return new KotMenuViewModel();
            }
        }
        public async Task<KotMenuViewModel> GetKotMenuItemsBasedOnCategoryAsync(int categoryId)
        {
            try
            {
                List<MenuItemsKot> menuItemsKot = new List<MenuItemsKot>();
                if (categoryId == -1)
                {
                    return await GetKotMenuAsync(null);
                }
                else if (categoryId == -2)
                {
                    menuItemsKot = await _context.CustomerFavourites
                        .Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true)
                        .Select(cf => new MenuItemsKot
                        {
                            Id = cf.ItemId,
                            Name = cf.Item.Name,
                            Price = cf.Item.Price,
                            Image = cf.Item.ImageUrl,
                            CategoryId = cf.Item.CategoryId,
                            ItemType = cf.Item.ItemType,
                            IsFavourite = !cf.IsDeleted
                        }).ToListAsync();
                }
                else
                {
                    menuItemsKot = await _context.Items
                        .Where(m => m.IsDeleted == false && m.IsAvailable == true && m.CategoryId == categoryId)
                        .Select(m => new MenuItemsKot
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Price = m.Price,
                            CategoryId = m.CategoryId,
                            Image = m.ImageUrl,
                            ItemType = m.ItemType,
                            IsFavourite = _context.CustomerFavourites
                                .FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
                        }).ToListAsync();
                }
                KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
                {
                    MenuItemsKot = menuItemsKot
                };
                return kotMenuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching KOT menu items based on category");
                Console.WriteLine(ex.Message);
                return new KotMenuViewModel();
            }
        }
        public async Task<KotMenuViewModel> SearchMenuItemsKotAsync(string search, int categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    return await GetKotMenuItemsBasedOnCategoryAsync(categoryId);
                }
                List<MenuItemsKot> menuItemsKot = new List<MenuItemsKot>();
                if (categoryId == -1)
                {
                    menuItemsKot = await _context.Items
                        .Where(m => m.IsDeleted == false && m.IsAvailable == true && m.Name.ToLower().Contains(search.ToLower()))
                        .Select(m => new MenuItemsKot
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Price = m.Price,
                            CategoryId = m.CategoryId,
                            Image = m.ImageUrl,
                            IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null,
                            ItemType = m.ItemType
                        }).ToListAsync();
                }
                else if (categoryId == -2)
                {
                    menuItemsKot = await _context.CustomerFavourites
                        .Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true && cf.Item.Name.ToLower().Contains(search.ToLower()))
                        .Select(cf => new MenuItemsKot
                        {
                            Id = cf.ItemId,
                            Name = cf.Item.Name,
                            Price = cf.Item.Price,
                            Image = cf.Item.ImageUrl,
                            CategoryId = cf.Item.CategoryId,
                            ItemType = cf.Item.ItemType,
                            IsFavourite = !cf.IsDeleted
                        }).ToListAsync();
                }
                else
                {
                    menuItemsKot = await _context.Items
                        .Where(m => m.IsDeleted == false && m.IsAvailable == true && m.CategoryId == categoryId && m.Name.ToLower().Contains(search.ToLower()))
                        .Select(m => new MenuItemsKot
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Price = m.Price,
                            CategoryId = m.CategoryId,
                            Image = m.ImageUrl,
                            ItemType = m.ItemType,
                            IsFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == m.Id && cf.IsDeleted == false) != null
                        }).ToListAsync();
                }
                List<ItemTaxes> itemTaxes = new List<ItemTaxes>();
                List<Item> itemList = await _context.Items.Where(i => i.IsDeleted == false && i.IsAvailable == true).ToListAsync();
                foreach (var item in itemList)
                {
                    ItemTaxes itemTaxes1 = new ItemTaxes
                    {
                        ItemId = item.Id,
                        IsDefault = item.DefaultTax ?? false,
                        TaxPercentage = (decimal)item.TaxPercentage,
                        ItemPrice = (decimal)item.Price
                    };
                    itemTaxes.Add(itemTaxes1);
                }
                KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
                {
                    MenuItemsKot = menuItemsKot,
                    ItemTaxes = itemTaxes,
                };
                return kotMenuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching menu items");
                Console.WriteLine(ex.Message);
                return new KotMenuViewModel();
            }
        }
        public async Task<JsonResult> AddToFavouritesAsync(int itemId, int userId)
        {
            try
            {
                CustomerFavourite customerFavourite = await _context.CustomerFavourites.FirstOrDefaultAsync(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite
                {
                    ItemId = -2348
                };
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
                    await _context.CustomerFavourites.AddAsync(customerFavourite2);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    customerFavourite.IsDeleted = false;
                    customerFavourite.UpdatedAt = DateTime.Now;
                    customerFavourite.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("Item with ID {ItemId} added to favourites by user with ID {UserId}", itemId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Item added to favourites successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding item with ID {ItemId} to favourites by user with ID {UserId}", itemId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding to favourites"
                });
            }
        }
        public async Task<JsonResult> DeleteFromFavouritesAsync(int itemId, int userId)
        {
            try
            {
                CustomerFavourite customerFavourite = await _context.CustomerFavourites.FirstOrDefaultAsync(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite();
                customerFavourite.IsDeleted = true;
                customerFavourite.UpdatedAt = DateTime.Now;
                customerFavourite.UpdatedBy = userId;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Item with ID {ItemId} removed from favourites by user with ID {UserId}", itemId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Item removed from favourites successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing item with ID {ItemId} from favourites by user with ID {UserId}", itemId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while removing from favourites"
                });
            }
        }
        public async Task<KotMenuViewModel> GetCustomerDetailsAsync(int orderId)
        {
            try
            {
                Customer customerDetails = await _context.Orders.Where(o => o.Id == orderId).OrderBy(o => o.Id).Select(o => o.Customer).LastOrDefaultAsync() ?? new Customer();
                WaitingListModal waitingListModalViewModel = new WaitingListModal
                {
                    Name = customerDetails.Name,
                    Email = customerDetails.Email ?? "",
                    MobileNumber = customerDetails.Phone ?? "",
                    NumberOfPeople = _context.OrderTableMappings.OrderBy(otm => otm.Id).LastOrDefault(o => o.OrderId == orderId).NoOfPersons,
                };
                KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
                {
                    WaitingListModal = waitingListModalViewModel
                };
                return kotMenuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customer details for order ID {OrderId}", orderId);
                Console.WriteLine(ex.Message);
                return new KotMenuViewModel();
            }
        }
        public async Task<JsonResult> UpdateCustomerDetailsAsync(WaitingListModal waitingListModal, int userId)
        {
            try
            {
                Customer customer1 = await _context.Customers.FirstOrDefaultAsync(c => c.Email == waitingListModal.Email) ?? new Customer();
                Order order = await _context.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync(o => o.CustomerId == customer1.Id) ?? new Order();
                List<OrderTableMapping> orderTableMappings = await _context.OrderTableMappings.Where(otm => otm.OrderId == order.Id).ToListAsync();
                int tableCapacity = 0;
                if (orderTableMappings.Count > 1)
                {
                    foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                    {
                        Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false);
                        if (table.Capacity > waitingListModal.NumberOfPeople)
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = "Customers can be managed in fewer tables than selected"
                            });
                        }
                    }
                }
                foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                {
                    Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false);
                    if (table != null)
                    {
                        tableCapacity += table.Capacity;
                    }
                }
                if (waitingListModal.NumberOfPeople > tableCapacity)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Number of people exceeds table capacity"
                    });
                }
                Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == waitingListModal.Email) ?? new Customer();
                {
                    customer.Name = waitingListModal.Name;
                    customer.Phone = waitingListModal.MobileNumber;
                    customer.Email = waitingListModal.Email;
                    customer.UpdatedAt = DateTime.Now;
                    customer.UpdatedBy = userId;
                }
                await _context.SaveChangesAsync();
                foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                {
                    OrderTableMapping orderTableMapping1 = await _context.OrderTableMappings.FirstOrDefaultAsync(otm => otm.Id == orderTableMapping.Id);
                    orderTableMapping1.NoOfPersons = waitingListModal.NumberOfPeople;
                    orderTableMapping1.UpdatedAt = DateTime.Now;
                    orderTableMapping1.UpdatedBy = userId;
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("Customer details updated successfully for order ID {OrderId} by user with ID {UserId}", order.Id, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Customer details updated successfully"
                });
            }
            catch (Exception ex)
            {
                Customer customer1 = await _context.Customers.FirstOrDefaultAsync(c => c.Email == waitingListModal.Email) ?? new Customer();
                Order order = await _context.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync(o => o.CustomerId == customer1.Id) ?? new Order();
                _logger.LogError(ex, "An error occurred while updating customer details for order ID {OrderId} by user with ID {UserId}", order.Id, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating customer details"
                });
            }
        }
        public async Task<KotMenuViewModel> GetSelectModifiersModalDataAsync(int itemId)
        {
            try
            {
                DAL.Models.Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.IsDeleted == false) ?? new DAL.Models.Item();
                AddModifiersModal addModifiersModal = new AddModifiersModal
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    ItemPrice = Math.Round((decimal)item.Price, 2),
                };
                List<ItemModifiergroup> itemModifierGroups = await _context.ItemModifiergroups.Where(im => im.ItemId == item.Id).ToListAsync();
                List<ModifierGroupsAddItem> modifierGroups = new List<ModifierGroupsAddItem>();
                foreach (ItemModifiergroup itemModifierGroup in itemModifierGroups)
                {
                    ModifierGroupsAddItem modifierGroup = new ModifierGroupsAddItem
                    {
                        ModifierGroupId = itemModifierGroup.ModifiergroupId,
                        ModifierGroupName = (await _context.ModifierGroups.FirstOrDefaultAsync(mg => mg.Id == itemModifierGroup.ModifiergroupId))?.Name,
                        MinSelection = itemModifierGroup.MinValue ?? 0,
                        MaxSelection = itemModifierGroup.MaxValue ?? 0
                    };
                    List<ModifierGroupItemsAddItem> modifierGroupItems = await _context.ModifierModifiergroupMappings
                        .Where(mg => mg.ModifiergroupId == itemModifierGroup.ModifiergroupId)
                        .Select(mg => new ModifierGroupItemsAddItem
                        {
                            ModifierId = mg.ModifierId,
                            ModifierName = mg.Modifier.Name,
                            Price = mg.Modifier.Price,
                        }).ToListAsync();
                    modifierGroup.ModifierGroupItems = modifierGroupItems;
                    modifierGroups.Add(modifierGroup);
                }
                addModifiersModal.ModifierGroups = modifierGroups;
                List<ItemTaxes> itemTaxes = new List<ItemTaxes>();
                List<Item> itemList = await _context.Items.Where(i => i.IsDeleted == false && i.IsAvailable == true).ToListAsync();
                foreach (var item2 in itemList)
                {
                    ItemTaxes itemTaxes1 = new ItemTaxes
                    {
                        ItemId = item2.Id,
                        IsDefault = item2.DefaultTax ?? false,
                        TaxPercentage = (decimal)item2.TaxPercentage,
                        ItemPrice = (decimal)item2.Price
                    };
                    itemTaxes.Add(itemTaxes1);
                }
                KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
                {
                    AddModifiersModal = addModifiersModal,
                    ItemTaxes = itemTaxes,
                };
                return kotMenuViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching select modifiers modal data for item ID {ItemId}", itemId);
                Console.WriteLine(ex.Message);
                return new KotMenuViewModel();
            }
        }
        public async Task<IActionResult> UpdateOrderAmountAsync(int orderId, int userId, float subTotal, float total)
        {
            try
            {
                Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
                order.SubTotal = (decimal?)subTotal;
                order.TotalAmount = (decimal)total;
                order.UpdatedAt = DateTime.Now;
                order.UpdatedBy = userId;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order total updated successfully for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Order total updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order total for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating order total"
                });
            }
        }
        public async Task<JsonResult> GetOrderWiseCommentAsync(int orderId)
        {
            try
            {
                Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
                if (order != null)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = order.Comment
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Order not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching order comment for order ID {OrderId}", orderId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while fetching order comment"
                });
            }
        }
        public async Task<JsonResult> GetItemWiseCommentAsync(int orderItemId)
        {
            try
            {
                OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.IsDeleted == false) ?? new OrderItem();
                if (orderItem != null)
                {
                    return new JsonResult(new
                    {
                        success = true,
                        message = orderItem.Comment
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Order item not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching order item comment for order item ID {OrderItemId}", orderItemId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while fetching order item comment"
                });
            }
        }
        public async Task<IActionResult> AddOrderWiseCommentAsync(int orderId, string comment, int userId)
        {
            try
            {
                Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
                if (order != null)
                {
                    order.Comment = comment;
                    order.UpdatedAt = DateTime.Now;
                    order.UpdatedBy = userId;
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Comment added successfully for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Comment added successfully"
                    });
                }
                else
                {
                    _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Order not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding comment for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding order comment"
                });
            }
        }
        public async Task<IActionResult> AddItemWiseCommentAsync(int orderItemId, string comment, int userId)
        {
            try
            {
                OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.IsDeleted == false) ?? new OrderItem();
                if (orderItem != null)
                {
                    orderItem.Comment = comment;
                    orderItem.UpdatedAt = DateTime.Now;
                    orderItem.UpdatedBy = userId;
                    _context.OrderItems.Update(orderItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Comment added successfully for order item ID {OrderItemId} by user with ID {UserId}", orderItemId, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Comment added successfully"
                    });
                }
                else
                {
                    _logger.LogWarning("Order item with ID {OrderItemId} not found", orderItemId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Order item not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding comment for order item ID {OrderItemId} by user with ID {UserId}", orderItemId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while adding order item comment"
                });
            }
        }
        public async Task<IActionResult> SaveOrderAsync(SaveOrderViewModel saveOrderViewModel, int userId)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == saveOrderViewModel.OrderId && oi.IsDeleted == false).ToListAsync();
                        if (saveOrderViewModel.OrderItems == null || saveOrderViewModel.OrderItems.Count == 0)
                        {
                            foreach (var orderItem in orderItems)
                            {
                                orderItem.IsDeleted = true;
                                orderItem.UpdatedAt = DateTime.Now;
                                orderItem.UpdatedBy = userId;
                                _context.OrderItems.Update(orderItem);
                                await _context.SaveChangesAsync();

                                List<OrderModifier> orderModifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false).ToListAsync();

                                if (orderModifiers != null && orderModifiers.Count > 0)
                                {
                                    foreach (var orderModifier in orderModifiers)
                                    {
                                        orderModifier.IsDeleted = true;
                                        orderModifier.UpdatedAt = DateTime.Now;
                                        orderModifier.UpdatedBy = userId;
                                        _context.OrderModifiers.Update(orderModifier);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                            List<OrderTaxis> orderTaxes2 = await _context.OrderTaxes.Where(ot => ot.OrderId == saveOrderViewModel.OrderId).ToListAsync();
                            if (orderTaxes2 != null && orderTaxes2.Count > 0)
                            {
                                foreach (var orderTax in orderTaxes2)
                                {
                                    orderTax.TaxAmount = 0;
                                    orderTax.UpdatedAt = DateTime.Now;
                                    orderTax.UpdatedBy = userId;
                                    _context.OrderTaxes.Update(orderTax);
                                    await _context.SaveChangesAsync();
                                }
                            }
                            await UpdateOrderAmountAsync(saveOrderViewModel.OrderId, userId, saveOrderViewModel.SubTotal, saveOrderViewModel.Total);
                            await transaction.CommitAsync();
                            _logger.LogInformation("Order saved successfully for order ID {OrderId} by user with ID {UserId}", saveOrderViewModel.OrderId, userId);
                            return new JsonResult(new
                            {
                                success = true,
                                message = "Order saved successfully"
                            });
                        }

                        foreach (var orderItem in saveOrderViewModel.OrderItems)
                        {
                            if (orderItem.OrderItemId == -1)
                            {
                                OrderItem newOrderItem = new OrderItem
                                {
                                    OrderId = saveOrderViewModel.OrderId,
                                    ItemId = orderItem.ItemId,
                                    Quantity = orderItem.Quantity,
                                    Price = (double?)_context.Items.FirstOrDefault(i => i.Id == orderItem.ItemId).Price,
                                    CreatedAt = DateTime.Now,
                                    IsDeleted = false,
                                    ReadyItemsCount = 0,
                                    CreatedBy = userId,
                                    UpdatedAt = DateTime.Now,
                                    UpdatedBy = userId,
                                };
                                await _context.OrderItems.AddAsync(newOrderItem);
                                await _context.SaveChangesAsync();
                                if (orderItem.ModifierIds != null && orderItem.ModifierIds.Count > 0)
                                {
                                    foreach (var id in orderItem.ModifierIds)
                                    {
                                        OrderModifier orderModifier = new OrderModifier
                                        {
                                            OrderItemId = newOrderItem.Id,
                                            ModifierId = id,
                                            Price = (double?)_context.Modifiers.FirstOrDefault(m => m.Id == id).Price,
                                            Quantity = orderItem.Quantity,
                                            CreatedAt = DateTime.Now,
                                            IsDeleted = false,
                                            CreatedBy = userId,
                                            UpdatedAt = DateTime.Now,
                                            UpdatedBy = userId,
                                        };
                                        await _context.OrderModifiers.AddAsync(orderModifier);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                            else
                            {
                                OrderItem existingOrderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItem.OrderItemId);
                                if (existingOrderItem != null)
                                {
                                    existingOrderItem.Quantity = orderItem.Quantity;
                                    existingOrderItem.UpdatedAt = DateTime.Now;
                                    existingOrderItem.UpdatedBy = userId;
                                    _context.OrderItems.Update(existingOrderItem);
                                    await _context.SaveChangesAsync();

                                    List<OrderModifier> existingOrderModifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.OrderItemId && om.IsDeleted == false).ToListAsync();
                                    if (existingOrderModifiers != null && existingOrderModifiers.Count > 0)
                                    {
                                        foreach (var existingOrderModifier in existingOrderModifiers)
                                        {
                                            existingOrderModifier.Quantity = orderItem.Quantity;
                                            existingOrderModifier.UpdatedAt = DateTime.Now;
                                            existingOrderModifier.UpdatedBy = userId;
                                            _context.OrderModifiers.Update(existingOrderModifier);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var orderItem in orderItems)
                        {
                            if (saveOrderViewModel.OrderItems.All(oi => oi.OrderItemId != orderItem.Id))
                            {
                                orderItem.IsDeleted = true;
                                orderItem.UpdatedAt = DateTime.Now;
                                orderItem.UpdatedBy = userId;
                                _context.OrderItems.Update(orderItem);
                                await _context.SaveChangesAsync();
                            }
                        }

                        List<OrderTaxis> orderTaxes = await _context.OrderTaxes.Where(ot => ot.OrderId == saveOrderViewModel.OrderId).ToListAsync();
                        if (orderTaxes.Count == 0)
                        {
                            foreach (var orderTax in saveOrderViewModel.OrderTaxes)
                            {
                                OrderTaxis orderTaxis = new OrderTaxis
                                {
                                    OrderId = saveOrderViewModel.OrderId,
                                    TaxId = orderTax.TaxName == "Other Tax" ? 0 : _context.TaxesFees.FirstOrDefault(tf => tf.Name == orderTax.TaxName).Id,
                                    TaxAmount = (decimal)orderTax.TaxAmount,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = userId,
                                    UpdatedAt = DateTime.Now,
                                    UpdatedBy = userId,
                                };
                                await _context.OrderTaxes.AddAsync(orderTaxis);
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            foreach (var orderTax in orderTaxes)
                            {
                                if (orderTax != null)
                                {
                                    if (orderTax.TaxId == 0)
                                    {
                                        orderTax.TaxAmount = (decimal)saveOrderViewModel.OrderTaxes.FirstOrDefault(ot => ot.TaxName == "Other Tax").TaxAmount;
                                        orderTax.UpdatedAt = DateTime.Now;
                                        orderTax.UpdatedBy = userId;
                                        _context.OrderTaxes.Update(orderTax);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        TaxesFee tax = await _context.TaxesFees.FirstOrDefaultAsync(tf => tf.Id == orderTax.TaxId);
                                        orderTax.TaxAmount = (decimal)saveOrderViewModel.OrderTaxes.FirstOrDefault(ot => ot.TaxName == tax.Name).TaxAmount;
                                        orderTax.UpdatedAt = DateTime.Now;
                                        orderTax.UpdatedBy = userId;
                                        _context.OrderTaxes.Update(orderTax);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        await UpdateOrderAmountAsync(saveOrderViewModel.OrderId, userId, saveOrderViewModel.SubTotal, saveOrderViewModel.Total);
                        await transaction.CommitAsync();
                        _logger.LogInformation("Order saved successfully for order ID {OrderId} by user with ID {UserId}", saveOrderViewModel.OrderId, userId);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Order saved successfully"
                        });
                    }
                    catch (Exception)
                    {
                        _logger.LogError("An error occurred while saving the order for order ID {OrderId} by user with ID {UserId}", saveOrderViewModel.OrderId, userId);
                        Console.WriteLine("An error occurred while saving the order");
                        await transaction.RollbackAsync();
                        return new JsonResult(new
                        {
                            success = false,
                            message = "An error occurred while saving the order"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the order for order ID {OrderId} by user with ID {UserId}", saveOrderViewModel.OrderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while saving the order"
                });
            }
        }
        public async Task<IActionResult> CompleteOrderAsync(int orderId, int userId)
        {
            try
            {
                bool canComplete = await _context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).AnyAsync(oi => oi.Quantity != oi.ReadyItemsCount);
                if (!canComplete)
                {
                    List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).ToListAsync();
                    if (orderItems == null || orderItems.Count == 0)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "No items found in the order, kindly cancel the order"
                        });
                    }
                    List<OrderTableMapping> orderTableMappings = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).ToListAsync();
                    foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                    {
                        Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false) ?? new Table();
                        if (table != null)
                        {
                            table.Status = "Available";
                            table.UpdatedAt = DateTime.Now;
                            table.UpdatedBy = userId;
                            _context.Tables.Update(table);
                            await _context.SaveChangesAsync();
                        }
                    }
                    Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
                    order.Status = "Completed";
                    order.PaymentMode = "Cash";
                    order.UpdatedAt = DateTime.Now;
                    order.UpdatedBy = userId;
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                    List<OrderTableMapping> orderTableMappings1 = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).ToListAsync();
                    foreach (OrderTableMapping orderTableMapping in orderTableMappings1)
                    {
                        orderTableMapping.IsDeleted = true;
                        orderTableMapping.UpdatedAt = DateTime.Now;
                        orderTableMapping.UpdatedBy = userId;
                        _context.OrderTableMappings.Update(orderTableMapping);
                        await _context.SaveChangesAsync();
                    }
                    Invoice invoice = new Invoice
                    {
                        OrderId = orderId,
                        InvoiceNo = "INV" + orderId.ToString()
                    };
                    await _context.Invoices.AddAsync(invoice);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Order completed successfully for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Order completed successfully"
                    });
                }
                else
                {
                    _logger.LogWarning("Cannot complete order ID {OrderId} as not all items are served", orderId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "All items must be served before completing the order"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while completing the order for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while completing the order"
                });
            }
        }
        public async Task<IActionResult> SaveCustomerReviewAsync(SaveCustomerReviewViewModel saveCustomerReviewViewModel, int userId)
        {
            try
            {
                CustomerReview customerReview = new CustomerReview
                {
                    OrderId = saveCustomerReviewViewModel.OrderId,
                    CustomerId = await _context.Orders.Where(o => o.Id == saveCustomerReviewViewModel.OrderId).Select(o => o.CustomerId).FirstOrDefaultAsync(),
                    Food = (short)saveCustomerReviewViewModel.FoodRating,
                    Ambience = (short)saveCustomerReviewViewModel.AmbienceRating,
                    Service = (short)saveCustomerReviewViewModel.ServiceRating,
                    AverageRating = (decimal)Math.Round((double)(saveCustomerReviewViewModel.FoodRating + saveCustomerReviewViewModel.AmbienceRating + saveCustomerReviewViewModel.ServiceRating) / 3, 1),
                    Comment = saveCustomerReviewViewModel.OrderReviewByCustomer,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                };
                await _context.CustomerReviews.AddAsync(customerReview);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Customer review saved successfully for order ID {OrderId} by user with ID {UserId}", saveCustomerReviewViewModel.OrderId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Review saved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving customer review for order ID {OrderId} by user with ID {UserId}", saveCustomerReviewViewModel.OrderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while saving the review"
                });
            }
        }
        public async Task<JsonResult> CanDeleteFromOrderAsync(int orderItemId)
        {
            try
            {
                if (orderItemId == -1)
                {
                    return new JsonResult(new
                    {
                        canDelete = true
                    });
                }
                OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.IsDeleted == false) ?? new OrderItem();
                if (orderItem.ReadyItemsCount > 0)
                {
                    return new JsonResult(new
                    {
                        canDelete = false
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        canDelete = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if item can be deleted from order item ID {OrderItemId}", orderItemId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    canDelete = false
                });
            }
        }
        public async Task<JsonResult> CanReduceFromOrderAsync(int orderItemId, int currentQuantity)
        {
            try
            {
                if (orderItemId == -1)
                {
                    return new JsonResult(new
                    {
                        canReduce = true
                    });
                }
                OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.IsDeleted == false) ?? new OrderItem();
                if (orderItem.ReadyItemsCount > currentQuantity - 1 && orderItem.ReadyItemsCount > 0)
                {
                    return new JsonResult(new
                    {
                        canReduce = false
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        canReduce = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if item can be reduced from order item ID {OrderItemId}", orderItemId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    canReduce = false
                });
            }
        }
        public async Task<JsonResult> AreModifiersSelectedAsync(int itemId)
        {
            try
            {
                int modifierGroupCount = await _context.ItemModifiergroups.Where(im => im.ItemId == itemId).CountAsync();
                if (modifierGroupCount > 0)
                {
                    return new JsonResult(new
                    {
                        areModifiersSelected = true
                    });
                }
                return new JsonResult(new
                {
                    areModifiersSelected = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if modifiers are selected for item ID {ItemId}", itemId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    areModifiersSelected = false
                });
            }
        }
        public async Task<IActionResult> CancelOrderAsync(int orderId, int userId)
        {
            try
            {
                List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).ToListAsync();
                if (orderItems.Count > 0)
                {
                    _logger.LogWarning("Cannot cancel order ID {OrderId} as items are already added", orderId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Order cannot be cancelled as items are already added"
                    });
                }
                Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
                order.Status = "Cancelled";
                order.UpdatedAt = DateTime.Now;
                order.UpdatedBy = userId;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                List<OrderTableMapping> orderTableMappings = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).ToListAsync();
                foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                {
                    Table table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false) ?? new Table();
                    if (table != null)
                    {
                        table.Status = "Available";
                        table.UpdatedAt = DateTime.Now;
                        table.UpdatedBy = userId;
                        _context.Tables.Update(table);
                        await _context.SaveChangesAsync();
                    }
                }
                foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                {
                    orderTableMapping.IsDeleted = true;
                    orderTableMapping.UpdatedAt = DateTime.Now;
                    orderTableMapping.UpdatedBy = userId;
                    _context.OrderTableMappings.Update(orderTableMapping);
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation("Order cancelled successfully for order ID {OrderId} by user with ID {UserId}", orderId, userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Order cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling order ID {OrderId} by user with ID {UserId}", orderId, userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while cancelling the order"
                });
            }
        }
    }
}