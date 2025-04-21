namespace BLL.Services
{
    using BLL.Interfaces;
    using DAL.DBContext;
    using DAL.Models;
    using DAL.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class KotService : IKotService
    {
        private readonly PizzaShopContext _context;
        public KotService(PizzaShopContext context)
        {
            _context = context;
        }
        public async Task<KotViewModel> GetKotViewModelAsync(int pageIndex = 1, int pageSize = 4, int? orderId = null)
        {
            List<Category>? categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            List<KotOrderCard> kotOrderCards = new List<KotOrderCard>();
            categories.Add(new Category
            {
                Id = 0,
                Name = "All",
                IsDeleted = false
            });
            List<Order> orders = new List<Order>();
            if (orderId != null)
            {
                orders = await _context.Orders.Where(o => o.Id == orderId && o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            else
            {
                orders = await _context.Orders.Where(o => o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            foreach (var order in orders)
            {
                List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id && oi.IsDeleted == false).ToListAsync();
                if (orderItems.Count > 0 && orderItems.Any(oi => oi.Quantity > oi.ReadyItemsCount))
                {
                    var createdAt = order?.CreatedAt ?? DateTime.Now;
                    KotOrderCard kotOrderCard = new KotOrderCard
                    {
                        OrderId = order?.Id ?? 0,
                        OrderDuration = (DateTime.Now - createdAt).Days > 0
                            ? $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : (DateTime.Now - createdAt).Hours > 0
                            ? $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : $"{(DateTime.Now - createdAt).Minutes} mins",
                        Section = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Section.Name)
                            .FirstOrDefaultAsync(),
                        Table = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Name)
                            .FirstOrDefaultAsync(),
                        OrderInstruction = order.Comment,
                    };
                    List<KotOrderCardItem> kotOrderCardItems = new List<KotOrderCardItem>();
                    foreach (var orderItem in orderItems)
                    {
                        var item = await _context.Items
                            .Where(i => i.Id == orderItem.ItemId && i.IsDeleted == false)
                            .Select(i => new
                            {
                                i.Id,
                                i.Name,
                                i.Price,
                                i.CategoryId
                            })
                            .FirstOrDefaultAsync();

                        if (item != null && (orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0)) > 0)
                        {
                            kotOrderCardItems.Add(new KotOrderCardItem
                            {
                                Id = item.Id,
                                ItemName = item.Name,
                                ItemQuantity = orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0),
                                ItemReadyItemsCount = orderItem.ReadyItemsCount ?? 0,
                                ItemInstruction = orderItem.Comment,
                                Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                    .Select(om => om.Modifier.Name)
                                    .ToListAsync(),
                            });
                        }
                        kotOrderCard.OrderItems = kotOrderCardItems;
                    }
                    kotOrderCards.Add(kotOrderCard);
                }
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            int TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / pageSize);
            if (pageIndex > TotalPages && TotalPages != 0)
            {
                pageIndex = TotalPages;
            }
            KotViewModel kotViewModel = new()
            {
                Categories = categories.OrderBy(c => c.Id).ToList(),
                PageSize = pageSize,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / 4),
                KotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            };
            return kotViewModel;
        }
        public async Task<KotViewModel> GetKotByCategoryAsync(int categoryId, int pageIndex, int pageSize, int? orderId = null)
        {
            if (categoryId == 0)
            {
                return await GetKotViewModelAsync(pageIndex, pageSize, orderId);
            }
            List<Category>? categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            List<KotOrderCard> kotOrderCards = new List<KotOrderCard>();
            categories.Add(new Category
            {
                Id = 0,
                Name = "All",
                IsDeleted = false
            });
            List<Order> orders = new List<Order>();
            if (orderId != null)
            {
                orders = await _context.Orders.Where(o => o.Id == orderId && o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            else
            {
                orders = await _context.Orders.Where(o => o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            foreach (var order in orders)
            {
                List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id && oi.IsDeleted == false && oi.Item.CategoryId == categoryId).ToListAsync();
                if (orderItems.Count > 0 && orderItems.Any(oi => oi.Quantity > oi.ReadyItemsCount))
                {
                    var createdAt = order?.CreatedAt ?? DateTime.Now;
                    KotOrderCard kotOrderCard = new KotOrderCard
                    {
                        OrderId = order?.Id ?? 0,
                        OrderDuration = (DateTime.Now - createdAt).Days > 0
                            ? $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : (DateTime.Now - createdAt).Hours > 0
                            ? $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : $"{(DateTime.Now - createdAt).Minutes} mins",
                        Section = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Section.Name)
                            .FirstOrDefaultAsync(),
                        Table = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Name)
                            .FirstOrDefaultAsync(),
                        OrderInstruction = order.Comment,
                    };
                    List<KotOrderCardItem> kotOrderCardItems = new List<KotOrderCardItem>();
                    foreach (var orderItem in orderItems)
                    {
                        var item = await _context.Items
                            .Where(i => i.Id == orderItem.ItemId && i.IsDeleted == false)
                            .Select(i => new
                            {
                                i.Id,
                                i.Name,
                                i.Price,
                                i.CategoryId
                            })
                            .FirstOrDefaultAsync();

                        if (item != null && (orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0)) > 0)
                        {
                            kotOrderCardItems.Add(new KotOrderCardItem
                            {
                                Id = item.Id,
                                ItemName = item.Name,
                                ItemQuantity = (int)(orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0)),
                                ItemInstruction = orderItem.Comment,
                                Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                    .Select(om => om.Modifier.Name)
                                    .ToListAsync(),
                            });
                        }
                        kotOrderCard.OrderItems = kotOrderCardItems;
                    }
                    kotOrderCards.Add(kotOrderCard);
                }
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            int TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / pageSize);
            if (pageIndex > TotalPages && TotalPages != 0)
            {
                pageIndex = TotalPages;
            }
            KotViewModel kotViewModel = new()
            {
                Categories = categories.OrderBy(c => c.Id).ToList(),
                TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / pageSize),
                KotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return kotViewModel;
        }
        public async Task<KotViewModel> GetMarkedAsPreparedModalAsync(int orderId, int categoryId, bool inReady)
        {
            KotViewModel kotViewModel = new();
            if (inReady)
            {
                kotViewModel = await GetReadyItems(categoryId, orderId);
            }
            else
            {
                kotViewModel = await GetKotByCategoryAsync(categoryId, 1, 4, orderId);
            }
            return kotViewModel;
        }
        public async Task<KotViewModel> MarkItemsAsReadyAsync(List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.IsDeleted == false) ?? new Order();
            if (order.Status == "Pending")
            {
                order.Status = "In Progress";
                order.UpdatedAt = DateTime.Now;
                order.UpdatedBy = userId;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            Table table = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId && otm.IsDeleted == false).Select(otm => otm.Table).FirstOrDefaultAsync() ?? new Table();
            if (table.Status == "Assigned")
            {
                table.Status = "Running";
                table.UpdatedAt = DateTime.Now;
                table.UpdatedBy = userId;
                _context.Tables.Update(table);
                await _context.SaveChangesAsync();
            }
            foreach (var item in readyItems)
            {
                OrderItem? orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.ItemId == item.Id && oi.OrderId == orderId && oi.IsDeleted == false);
                if (orderItem != null)
                {
                    orderItem.ReadyItemsCount = orderItem.ReadyItemsCount + item.Quantity;
                    orderItem.UpdatedAt = DateTime.Now;
                    orderItem.UpdatedBy = userId;
                    _context.OrderItems.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
            }
            KotViewModel kotViewModel = await GetKotByCategoryAsync(categoryId, 1, 4);
            return kotViewModel;
        }

        public async Task<KotViewModel> MarkItemsAsInPrepared(List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId)
        {
            foreach (var item in readyItems)
            {
                OrderItem? orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.ItemId == item.Id && oi.OrderId == orderId && oi.IsDeleted == false);
                if (orderItem != null)
                {
                    orderItem.ReadyItemsCount = orderItem.ReadyItemsCount - item.Quantity;
                    orderItem.UpdatedAt = DateTime.Now;
                    orderItem.UpdatedBy = userId;
                    _context.OrderItems.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
            }
            KotViewModel kotViewModel = await GetReadyItems(categoryId);
            return kotViewModel;
        }
        public async Task<KotViewModel> GetReadyItems(int categoryId, int pageIndex = 1, int? orderId = null)
        {
            List<Category>? categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            List<KotOrderCard> kotOrderCards = new List<KotOrderCard>();
            categories.Add(new Category
            {
                Id = 0,
                Name = "All",
                IsDeleted = false
            });
            List<Order> orders = new List<Order>();
            if (orderId != null)
            {
                orders = await _context.Orders.Where(o => o.Id == orderId && o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            else
            {
                orders = await _context.Orders.Where(o => o.IsDeleted == false && o.Status != "Completed").ToListAsync();
            }
            foreach (var order in orders)
            {
                List<OrderItem> orderItems = new List<OrderItem>();
                if (categoryId == 0)
                {
                    orderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id && oi.IsDeleted == false).ToListAsync();
                }
                else
                {
                    orderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id && oi.IsDeleted == false && oi.Item.CategoryId == categoryId).ToListAsync();
                }
                if (orderItems.Count > 0 && orderItems.Any(oi => oi.ReadyItemsCount > 0))
                {
                    var createdAt = order?.CreatedAt ?? DateTime.Now;
                    KotOrderCard kotOrderCard = new KotOrderCard
                    {
                        OrderId = order?.Id ?? 0,
                        OrderDuration = (DateTime.Now - createdAt).Days > 0
                            ? $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : (DateTime.Now - createdAt).Hours > 0
                            ? $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                            : $"{(DateTime.Now - createdAt).Minutes} mins",
                        Section = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Section.Name)
                            .FirstOrDefaultAsync(),
                        Table = await _context.OrderTableMappings
                            .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                            .Select(otm => otm.Table.Name)
                            .FirstOrDefaultAsync(),
                        OrderInstruction = order.Comment,
                    };
                    List<KotOrderCardItem> kotOrderCardItems = new List<KotOrderCardItem>();
                    foreach (var orderItem in orderItems)
                    {
                        var item = await _context.Items
                            .Where(i => i.Id == orderItem.ItemId && i.IsDeleted == false)
                            .Select(i => new
                            {
                                i.Id,
                                i.Name,
                                i.Price,
                                i.CategoryId
                            })
                            .FirstOrDefaultAsync();

                        if (item != null && orderItem.ReadyItemsCount > 0)
                        {
                            kotOrderCardItems.Add(new KotOrderCardItem
                            {
                                Id = item.Id,
                                ItemName = item.Name,
                                ItemQuantity = orderItem.ReadyItemsCount ?? 0,
                                ItemInstruction = orderItem.Comment,
                                Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                    .Select(om => om.Modifier.Name)
                                    .ToListAsync(),
                            });
                        }
                        kotOrderCard.OrderItems = kotOrderCardItems;
                    }
                    kotOrderCards.Add(kotOrderCard);
                }
            }
            if (categoryId != 0)
            {
                kotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).ToList();
            }
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            int TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / 4);
            if (pageIndex > TotalPages && TotalPages != 0)
            {
                pageIndex = TotalPages;
            }
            KotViewModel kotViewModel = new()
            {
                Categories = categories.OrderBy(c => c.Id).ToList(),
                KotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).Skip((pageIndex - 1) * 4).Take(4).ToList(),
                PageSize = 4,
                PageIndex = pageIndex,
                TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / 4),
            };
            return kotViewModel;
        }
    }
}