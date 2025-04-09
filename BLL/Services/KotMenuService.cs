using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class KotMenuService : IKotMenuService
    {
        private readonly PizzaShopContext _context;
        public KotMenuService(PizzaShopContext context)
        {
            _context = context;
        }
        public async Task<KotMenuViewModel> GetKotMenuAsync(int? orderId)
        {
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
                        ItemId = oi.ItemId,
                        ItemName = oi.Item.Name,
                        ItemQuantity = oi.Quantity,
                        ItemTotalPrice = (decimal?)(oi.Price * oi.Quantity),
                        Modifiers = _context.OrderModifiers.Where(om => om.OrderItemId == oi.Id && om.IsDeleted == false).Select(om => new ModifierDetails
                        {
                            ModifierName = om.Modifier.Name,
                            ModifierPrice = (decimal?)om.Price
                        }).ToList(),
                        ModifiersTotalPrice = (decimal?)_context.OrderModifiers.Where(om => om.OrderItemId == oi.Id && om.IsDeleted == false).Sum(om => om.Price * om.Quantity),
                    }).ToListAsync();

                    orderDetailsCard.SectionName = sectionName;
                    orderDetailsCard.TableNames = tableNames;
                    orderDetailsCard.OrderItemDetails = orderItemDetails;
                    orderDetailsCard.SubTotal = (decimal?)_context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).Sum(oi => oi.Price * oi.Quantity) ?? 0;
                    orderDetailsCard.SubTotal += (decimal?)_context.OrderModifiers.Where(om => om.OrderItem.OrderId == orderId && om.IsDeleted == false).Sum(om => om.Price * om.Quantity) ?? 0;
                    orderDetailsCard.SubTotal = Math.Round((decimal)orderDetailsCard.SubTotal, 2);
                    orderDetailsCard.Taxes = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).Select(ot => new InvoiceTax
                    {
                        TaxName = _context.TaxesFees.Any(tf => tf.Id == ot.TaxId)
                            ? _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).Name
                            : null,
                        TaxAmount = _context.TaxesFees.Any(tf => tf.Id == ot.TaxId)
                            ? _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).TaxType == "Percentage"
                                ? (double)Math.Round((decimal)orderDetailsCard.SubTotal * (decimal)ot.TaxAmount / 100, 2)
                                : (double)Math.Round((decimal)ot.TaxAmount, 2)
                            : 0
                    }).ToListAsync();
                    orderDetailsCard.TotalPrice = (decimal?)_context.Orders.Where(o => o.Id == orderId).Select(o => o.TotalAmount).FirstOrDefault() ?? 0;
                }
            }

            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                Categories = categories,
                MenuItemsKot = menuItemsKot,
                OrderDetailsCard = orderDetailsCard
            };

            return kotMenuViewModel;
        }

        public async Task<KotMenuViewModel> GetKotMenuItemsBasedOnCategoryAsync(int categoryId)
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

        public async Task<KotMenuViewModel> SearchMenuItemsKotAsync(string search, int categoryId)
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
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                MenuItemsKot = menuItemsKot
            };
            return kotMenuViewModel;
        }

        public async Task<JsonResult> AddToFavouritesAsync(int itemId, int userId)
        {
            CustomerFavourite customerFavourite = await _context.CustomerFavourites.FirstOrDefaultAsync(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite { ItemId = -2348 };
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
            return new JsonResult(new { success = true, message = "Item added to favourites successfully" });
        }

        public async Task<JsonResult> DeleteFromFavouritesAsync(int itemId, int userId)
        {
            CustomerFavourite customerFavourite = await _context.CustomerFavourites.FirstOrDefaultAsync(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite();
            customerFavourite.IsDeleted = true;
            customerFavourite.UpdatedAt = DateTime.Now;
            customerFavourite.UpdatedBy = userId;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Item removed from favourites successfully" });
        }

        public async Task<KotMenuViewModel> GetCustomerDetailsAsync(int orderId)
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

        public async Task<JsonResult> UpdateCustomerDetailsAsync(WaitingListModal waitingListModal, int userId)
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
                        return new JsonResult(new { success = false, message = "Customers can be managed in fewer tables than selected" });
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
                return new JsonResult(new { success = false, message = "Number of people exceeds table capacity" });
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
            return new JsonResult(new { success = true, message = "Customer details updated successfully" });
        }

        public async Task<KotMenuViewModel> GetSelectModifiersModalDataAsync(int itemId)
        {
            DAL.Models.Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.IsDeleted == false) ?? new DAL.Models.Item();
            AddModifiersModal addModifiersModal = new AddModifiersModal
            {
                ItemId = item.Id,
                ItemName = item.Name
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
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                AddModifiersModal = addModifiersModal
            };
            return kotMenuViewModel;
        }

        public async Task<IActionResult> AddItemToOrderAsync(int itemId, int orderId, List<int> modifierIds, int userId)
        {
            DAL.Models.Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.IsDeleted == false) ?? new DAL.Models.Item();
            item.Quantity -= 1;
            if (item.Quantity <= 0)
            {
                item.IsAvailable = false;
            }
            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            OrderItem orderItem = new OrderItem
            {
                OrderId = orderId,
                ItemId = itemId,
                Quantity = 1,
                Price = (double?)item.Price,
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedBy = userId,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };
            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();

            foreach (int modifierId in modifierIds)
            {
                Modifier modifier = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == modifierId) ?? new Modifier();
                modifier.Quantity -= 1;
                modifier.UpdatedAt = DateTime.Now;
                modifier.UpdatedBy = userId;
                _context.Modifiers.Update(modifier);
                await _context.SaveChangesAsync();

                OrderModifier orderItemModifier = new OrderModifier
                {
                    OrderItemId = orderItem.Id,
                    ModifierId = modifierId,
                    Price = (double?)modifier.Price,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false,
                    Quantity = 1
                };
                await _context.OrderModifiers.AddAsync(orderItemModifier);
                await _context.SaveChangesAsync();
            }

            List<OrderTaxis> orderTaxes = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToListAsync();
            double SubTotal = 0, totalTax = 0;

            if (orderTaxes.Count == 0)
            {
                List<TaxesFee> taxesFees = await _context.TaxesFees.Where(tf => tf.IsDeleted == false).ToListAsync();
                foreach (TaxesFee taxesFee in taxesFees)
                {
                    OrderTaxis orderTaxis = new OrderTaxis
                    {
                        OrderId = orderId,
                        TaxId = taxesFee.Id,
                        TaxAmount = taxesFee.Amount,
                        CreatedBy = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedBy = userId,
                        UpdatedAt = DateTime.Now,
                    };
                    await _context.OrderTaxes.AddAsync(orderTaxis);
                    await _context.SaveChangesAsync();
                }
            }
            await UpdateOrderAmount(orderId, userId);
            return new JsonResult(new { success = true, message = "Item added to order successfully" });
        }
        public async Task<IActionResult> DeleteItemFromOrderAsync(int orderId, int itemId, int userId)
        {
            Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId) ?? new Item();
            OrderItem ordeItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ItemId == itemId) ?? new OrderItem();
            item.Quantity += ordeItem.Quantity;
            item.UpdatedBy = userId;
            item.UpdatedAt = DateTime.Now;
            ordeItem.IsDeleted = true;
            ordeItem.UpdatedAt = DateTime.Now;
            ordeItem.UpdatedBy = userId;
            _context.OrderItems.Update(ordeItem);
            await _context.SaveChangesAsync();
            List<OrderModifier> orderModifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == ordeItem.Id && om.IsDeleted == false).ToListAsync();
            foreach (OrderModifier orderModifier in orderModifiers)
            {
                orderModifier.IsDeleted = true;
                orderModifier.UpdatedAt = DateTime.Now;
                orderModifier.UpdatedBy = userId;
                _context.Update(orderModifier);
                await _context.SaveChangesAsync();
            }
            await UpdateOrderAmount(orderId, userId);
            return new JsonResult(new { success = true, message = "Item deleted from order successfully." });
        }
        public async Task<KotMenuViewModel> RefreshOrderItemDetails(int orderId)
        {
            OrderDetailsCard orderDetailsCard = new OrderDetailsCard();
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
                    ItemId = oi.ItemId,
                    ItemName = oi.Item.Name,
                    ItemQuantity = oi.Quantity,
                    ItemTotalPrice = (decimal?)(oi.Price * oi.Quantity),
                    Modifiers = _context.OrderModifiers.Where(om => om.OrderItemId == oi.Id).Select(om => new ModifierDetails
                    {
                        ModifierName = om.Modifier.Name,
                        ModifierPrice = (decimal?)om.Price
                    }).ToList(),
                    ModifiersTotalPrice = (decimal?)_context.OrderModifiers.Where(om => om.OrderItemId == oi.Id).Sum(om => om.Price * om.Quantity),
                }).ToListAsync();

                orderDetailsCard.SectionName = sectionName;
                orderDetailsCard.TableNames = tableNames;
                orderDetailsCard.OrderItemDetails = orderItemDetails;
                orderDetailsCard.SubTotal = (decimal?)_context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).Sum(oi => oi.Price * oi.Quantity) ?? 0;
                orderDetailsCard.SubTotal += (decimal?)_context.OrderModifiers.Where(om => om.OrderItem.OrderId == orderId && om.IsDeleted == false).Sum(om => om.Price * om.Quantity) ?? 0;
                orderDetailsCard.SubTotal = Math.Round((decimal)orderDetailsCard.SubTotal, 2);
                orderDetailsCard.Taxes = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).Select(ot => new InvoiceTax
                {
                    TaxName = _context.TaxesFees.Any(tf => tf.Id == ot.TaxId)
                        ? _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).Name
                        : null,
                    TaxAmount = _context.TaxesFees.Any(tf => tf.Id == ot.TaxId)
                        ? _context.TaxesFees.FirstOrDefault(tf => tf.Id == ot.TaxId).TaxType == "Percentage"
                            ? (double)Math.Round((decimal)orderDetailsCard.SubTotal * (decimal)ot.TaxAmount / 100, 2)
                            : (double)Math.Round((decimal)ot.TaxAmount, 2)
                        : 0
                }).ToListAsync();
                orderDetailsCard.TotalPrice = (decimal?)_context.Orders.Where(o => o.Id == orderId).Select(o => o.TotalAmount).FirstOrDefault() ?? 0;
            }
            KotMenuViewModel kotMenuViewModel = new KotMenuViewModel
            {
                OrderDetailsCard = orderDetailsCard
            };
            return kotMenuViewModel;
        }
        public async Task<JsonResult> IncreaseOrderItemQuantity(int orderId, int itemId, int userId)
        {
            OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ItemId == itemId) ?? new OrderItem();
            List<OrderModifier> orderModifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false).ToListAsync() ?? new List<OrderModifier>();
            Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId) ?? new Item();
            if (item.Quantity <= 0)
            {
                return new JsonResult(new { success = false, message = "Item not available" });
            }
            item.Quantity -= 1;
            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            orderItem.Quantity += 1;
            orderItem.UpdatedAt = DateTime.Now;
            orderItem.UpdatedBy = userId;
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
            foreach (OrderModifier orderModifier in orderModifiers)
            {
                orderModifier.Quantity += 1;
                orderModifier.UpdatedAt = DateTime.Now;
                orderModifier.UpdatedBy = userId;
                _context.OrderModifiers.Update(orderModifier);
                await _context.SaveChangesAsync();
            }
            await UpdateOrderAmount(orderId, userId);
            return new JsonResult(new { success = true, message = "Item added successfully" });
        }
        public async Task<JsonResult> DecreaseOrderItemQuantity(int orderId, int itemId, int userId)
        {
            OrderItem orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ItemId == itemId) ?? new OrderItem();
            List<OrderModifier> orderModifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false).ToListAsync() ?? new List<OrderModifier>();
            Item item = await _context.Items.FirstOrDefaultAsync(i => i.Id == itemId) ?? new Item();
            item.Quantity += 1;
            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            orderItem.Quantity -= 1;
            if (orderItem.Quantity == 0)
            {
                orderItem.IsDeleted = true;
                orderItem.UpdatedAt = DateTime.Now;
                orderItem.UpdatedBy = userId;
                foreach (OrderModifier orderModifier in orderModifiers)
                {
                    orderModifier.Quantity -= 1;
                    orderModifier.IsDeleted = true;
                    orderModifier.UpdatedAt = DateTime.Now;
                    orderModifier.UpdatedBy = userId;
                    _context.OrderModifiers.Update(orderModifier);
                    await _context.SaveChangesAsync();
                }
                await UpdateOrderAmount(orderId, userId);
                return new JsonResult(new { success = true, message = "Item removed successfully" });
            }
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
            foreach (OrderModifier orderModifier in orderModifiers)
            {
                orderModifier.Quantity -= 1;
                orderModifier.UpdatedAt = DateTime.Now;
                orderModifier.UpdatedBy = userId;
                _context.OrderModifiers.Update(orderModifier);
                await _context.SaveChangesAsync();
            }
            await UpdateOrderAmount(orderId, userId);
            return new JsonResult(new { success = true, message = "Item removed successfully" });
        }

        public async Task<IActionResult> UpdateOrderAmount(int orderId, int userId)
        {
            double SubTotal = 0, totalTax = 0;
            SubTotal = await _context.OrderItems.Where(oi => oi.OrderId == orderId && oi.IsDeleted == false).SumAsync(oi => oi.Price * oi.Quantity) ?? 0;
            SubTotal += await _context.OrderModifiers.Where(om => om.OrderItem.OrderId == orderId && om.IsDeleted == false).SumAsync(om => om.Price * om.Quantity) ?? 0;

            List<OrderTaxis> orderTaxis2 = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToListAsync();
            foreach (OrderTaxis orderTaxis1 in orderTaxis2)
            {
                string taxType = (await _context.TaxesFees.FirstOrDefaultAsync(tf => tf.Id == orderTaxis1.TaxId))?.TaxType ?? "";
                if (taxType == "Percentage")
                {
                    totalTax += SubTotal * (double)orderTaxis1.TaxAmount / 100;
                }
                else if (taxType == "Fixed Amount")
                {
                    totalTax += (double)orderTaxis1.TaxAmount;
                }
            }
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
            order.TotalAmount = (decimal)(SubTotal + totalTax);
            order.TotalAmount = Math.Round(order.TotalAmount, 2);
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = userId;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Order total updated successfully" });
        }
        public async Task<JsonResult> GetOrderWiseCommentAsync (int orderId)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
            if (order != null)
            {
                return new JsonResult(new { success = true, message = order.Comment });
            }
            else 
            {
                return new JsonResult(new { success = false, message = "Order not found" });
            }
        }
        public async Task<IActionResult> AddOrderWiseComment (int orderId, string comment, int userId)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId) ?? new Order();
            if (order != null)
            {
                order.Comment = comment;
                order.UpdatedAt = DateTime.Now;
                order.UpdatedBy = userId;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Comment added successfully" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Order not found" });
            }
        }
    }
}