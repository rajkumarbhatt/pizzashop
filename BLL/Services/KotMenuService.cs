using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class KotMenuService : IKotMenuService
    {
        private readonly PizzaShopContext _context;
        public KotMenuService(PizzaShopContext context)
        {
            _context = context;
        }
        public KotMenuViewModel GetKotMenu(int? orderId )
        {
            OrderDetailsCard orderDetailsCard = new OrderDetailsCard();
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
            if (orderId != null)
            {
                List<int> tableIds = _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).Select(otb => otb.TableId).ToList();
                int sectionId = _context.Tables.FirstOrDefault(t => t.Id == tableIds[0]).SectionId;
                string sectionName = _context.Sections.FirstOrDefault(s => s.Id == sectionId).Name;
                string tableNames = "";
                foreach (int tableId in tableIds)
                {
                    string tableName = _context.Tables.FirstOrDefault(t => t.Id == tableId).Name;
                    if (tableNames == "")
                    {
                        tableNames = tableName;
                    }
                    else
                    {
                        tableNames += ", " + tableName;
                    }
                    List<OrderItemDetials> orderItemDetails = _context.OrderItems.Where(oi => oi.OrderId == orderId).Select(oi => new OrderItemDetials
                    {
                        ItemName = oi.Item.Name,
                        ItemQuantity = oi.Quantity,
                        ItemTotalPrice = (decimal?)(oi.Price * oi.Quantity),
                        Modifiers = _context.OrderModifiers.Where(om => om.OrderItemId == oi.Id).Select(om => new ModifierDetails
                        {
                            ModifierName = om.Modifier.Name,
                            ModifierPrice = (decimal?)om.Price
                        }).ToList(),
                        ModifiersTotalPrice = (decimal?)_context.OrderModifiers.Where(om => om.OrderItemId == oi.Id).Sum(om => om.Price),
                    }).ToList();
                    orderDetailsCard.SectionName = sectionName;
                    orderDetailsCard.TableNames = tableNames;
                    orderDetailsCard.OrderItemDetails = orderItemDetails;
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

        public KotMenuViewModel GetKotMenuItemsBasedOnCategory(int categoryId)
        {
            List<MenuItemsKot> menuItemsKot = new List<MenuItemsKot>();
            if (categoryId == -1)
            {
                return GetKotMenu(null);
            }
            else if (categoryId == -2)
            {
                menuItemsKot = _context.CustomerFavourites.Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true).Select(cf => new MenuItemsKot
                {
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
                menuItemsKot = _context.CustomerFavourites.Where(cf => cf.IsDeleted == false && cf.Item.IsAvailable == true && cf.Item.Name.ToLower().Contains(search.ToLower())).Select(cf => new MenuItemsKot
                {
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
            CustomerFavourite customerFavourite = _context.CustomerFavourites.FirstOrDefault(cf => cf.ItemId == itemId && cf.IsDeleted == false) ?? new CustomerFavourite { ItemId = -2348 };
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

        public KotMenuViewModel GetCustomerDetails(int orderId)
        {
            Customer customerDetails = _context.Orders.Where(o => o.Id == orderId).OrderBy(o => o.Id).Select(o => o.Customer).LastOrDefault() ?? new Customer();
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

        public JsonResult UpdateCustomerDetails(WaitingListModal waitingListModal, int userId)
        {
            Customer customer1 = _context.Customers.FirstOrDefault(c => c.Email == waitingListModal.Email) ?? new Customer();
            Order order = _context.Orders.OrderByDescending(o => o.Id).FirstOrDefault(o => o.CustomerId == customer1.Id) ?? new Order();
            List<OrderTableMapping> orderTableMappings = _context.OrderTableMappings.Where(otm => otm.OrderId == order.Id).ToList();
            int tableCapacity = 0;
            if (orderTableMappings.Count > 1)
            {
                foreach (OrderTableMapping orderTableMapping in orderTableMappings)
                {
                    Table table = _context.Tables.FirstOrDefault(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false);
                    if (table.Capacity > waitingListModal.NumberOfPeople)
                    {
                        return new JsonResult(new { success = false, message = "Customers can be manages in less than selected tables" });
                    }
                }
            }
            foreach (OrderTableMapping orderTableMapping in orderTableMappings)
            {
                Table table = _context.Tables.FirstOrDefault(t => t.Id == orderTableMapping.TableId && t.IsDeleted == false);
                if (table != null)
                {
                    tableCapacity += table.Capacity;
                }
            }
            if (waitingListModal.NumberOfPeople > tableCapacity)
            {
                return new JsonResult(new { success = false, message = "Number of people exceeds table capacity" });
            }
            Customer customer = _context.Customers.FirstOrDefault(c => c.Email == waitingListModal.Email) ?? new Customer();
            {
                customer.Name = waitingListModal.Name;
                customer.Phone = waitingListModal.MobileNumber;
                customer.Email = waitingListModal.Email;
                customer.UpdatedAt = DateTime.Now;
                customer.UpdatedBy = userId;
            }
            _context.SaveChanges();
            foreach (OrderTableMapping orderTableMapping in orderTableMappings)
            {
                OrderTableMapping orderTableMapping1 = _context.OrderTableMappings.FirstOrDefault(otm => otm.Id == orderTableMapping.Id);
                orderTableMapping1.NoOfPersons = waitingListModal.NumberOfPeople;
                orderTableMapping1.UpdatedAt = DateTime.Now;
                orderTableMapping1.UpdatedBy = userId;
                _context.SaveChanges();
            }
            return new JsonResult(new { success = true, message = "Customer details updated successfully" });
        }

        public KotMenuViewModel GetSelectModifiersModalData(int itemId)
        {
            DAL.Models.Item item = _context.Items.FirstOrDefault(i => i.Id == itemId && i.IsDeleted == false) ?? new DAL.Models.Item();
            AddModifiersModal addModifiersModal = new AddModifiersModal();
            addModifiersModal.ItemId = item.Id;
            addModifiersModal.ItemName = item.Name;
            List<ItemModifiergroup> itemModifierGroups = _context.ItemModifiergroups.Where(im => im.ItemId == item.Id).ToList();
            List<ModifierGroupsAddItem> modifierGroups = new List<ModifierGroupsAddItem>();
            foreach (ItemModifiergroup itemModifierGroup in itemModifierGroups)
            {
                ModifierGroupsAddItem modifierGroup = new ModifierGroupsAddItem();
                modifierGroup.ModifierGroupId = itemModifierGroup.ModifiergroupId;
                modifierGroup.ModifierGroupName = _context.ModifierGroups.FirstOrDefault(mg => mg.Id == itemModifierGroup.ModifiergroupId)?.Name;
                modifierGroup.MinSelection = itemModifierGroup.MinValue ?? 0;
                modifierGroup.MaxSelection = itemModifierGroup.MaxValue ?? 0;
                List<ModifierGroupItemsAddItem> modifierGroupItems = _context.ModifierModifiergroupMappings.Where(mg => mg.ModifiergroupId == itemModifierGroup.ModifiergroupId).Select(mg => new ModifierGroupItemsAddItem
                {
                    ModifierId = mg.ModifierId,
                    ModifierName = mg.Modifier.Name,
                    Price = mg.Modifier.Price,
                }).ToList();
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

        public IActionResult AddItemToOrder(int itemId, int orderId, List<int> modifierIds, int userId)
        {
            DAL.Models.Item item = _context.Items.FirstOrDefault(i => i.Id == itemId && i.IsDeleted == false) ?? new DAL.Models.Item();
            item.Quantity -= 1;
            if (item.Quantity <= 0)
            {
                item.IsAvailable = false;
            }
            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _context.Items.Update(item);
            _context.SaveChanges();
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
            };
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
            foreach (int modifierId in modifierIds)
            {
                Modifier modifier = _context.Modifiers.FirstOrDefault(m => m.Id == modifierId) ?? new Modifier();
                modifier.Quantity -= 1;
                modifier.UpdatedAt = DateTime.Now;
                modifier.UpdatedBy = userId;
                _context.Modifiers.Update(modifier);
                _context.SaveChanges();
                OrderModifier orderItemModifier = new OrderModifier
                {
                    OrderItemId = orderItem.Id,
                    ModifierId = modifierId,
                    Price = (double?)modifier.Price,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedBy = userId,
                    UpdatedAt = DateTime.Now,
                };
                _context.OrderModifiers.Add(orderItemModifier);
                _context.SaveChanges();
            }
            List<OrderTaxis> orderTaxes = _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToList();
            double SubTotal = 0, totalTax = 0;
            if (orderTaxes.Count == 0)
            {
                List<TaxesFee> taxesFees = _context.TaxesFees.Where(tf => tf.IsDeleted == false).ToList();
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
                    _context.OrderTaxes.Add(orderTaxis);
                    _context.SaveChanges();
                }
            }
            SubTotal = _context.OrderItems.Where(oi => oi.OrderId == orderId).Sum(oi => oi.Price * oi.Quantity) ?? 0;
            SubTotal += _context.OrderModifiers.Where(om => om.OrderItem.OrderId == orderId).Sum(om => om.Price) ?? 0;
            List<OrderTaxis> orderTaxis2 = _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToList();
            foreach (OrderTaxis orderTaxis1 in orderTaxis2)
            {
                
                string taxType = _context.TaxesFees.FirstOrDefault(tf => tf.Id == orderTaxis1.TaxId)?.TaxType ?? "";
                if (taxType == "Percentage")
                {
                    totalTax += SubTotal * (double)orderTaxis1.TaxAmount / 100;
                }
                else if (taxType == "Fixed Amount")
                {
                    totalTax += (double)orderTaxis1.TaxAmount;
                }
            }
            totalTax = Math.Round(totalTax, 2);
            Order order = _context.Orders.FirstOrDefault(o => o.Id == orderId) ?? new Order();
            order.TotalAmount = (decimal)(SubTotal + totalTax);
            order.UpdatedAt = DateTime.Now;
            order.UpdatedBy = userId;
            _context.Orders.Update(order);
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Item added to order successfully" });
        }
    }
}