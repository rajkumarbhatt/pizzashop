using System.Text.Json;
using BLL.Hubs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class KotService : IKotService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<KotService> _logger;
        private readonly IHubContext<KOTHub> _hubContext;
        public KotService(PizzaShopContext context, ILogger<KotService> logger, IHubContext<KOTHub> hubContext)
        {
            _hubContext = hubContext;
            _logger = logger;
            _context = context;
        }
        public async Task<KotViewModel> GetKotViewModelAsync(int pageIndex = 1, int pageSize = 4, int? orderId = null)
        {
            try
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
                            OrderDuration = (DateTime.Now - createdAt).Days > 0 ?
                                $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Hours > 0 ?
                                $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Minutes > 0 ?
                                $"{(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                $"{(DateTime.Now - createdAt).Seconds} secs",
                            Section = await _context.OrderTableMappings
                                .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                .Select(otm => otm.Table.Section.Name)
                                .FirstOrDefaultAsync(),
                            Table = string.Join(", ", await _context.OrderTableMappings
                                    .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                    .Select(otm => otm.Table.Name)
                                    .ToListAsync()),
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
                                    OrderItemId = orderItem.Id,
                                    Id = item.Id,
                                    ItemName = item.Name,
                                    ItemQuantity = orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0),
                                    ItemReadyItemsCount = orderItem.ReadyItemsCount ?? 0,
                                    ItemInstruction = orderItem.Comment,
                                    Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                        .Select(om => new ModifierDetails
                                        {
                                            ModifierId = om.ModifierId,
                                            ModifierName = om.Modifier.Name,
                                        })
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
                if (pageIndex > TotalPages && TotalPages != 0 && orderId == null)
                {
                    pageIndex = TotalPages;
                }
                if (orderId != null)
                {
                    kotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).ToList();
                }
                else
                {
                    kotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                KotViewModel kotViewModel = new()
                {
                    Categories = categories.OrderBy(c => c.Id).ToList(),
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / 4),
                    KotOrderCards = kotOrderCards
                };
                return kotViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the KOT view model");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while fetching the KOT view model", ex);
            }
        }
        public async Task<KotViewModel> GetKotByCategoryAsync(int categoryId, int pageIndex, int pageSize, int? orderId = null)
        {
            try
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
                            OrderDuration = (DateTime.Now - createdAt).Days > 0 ?
                                $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Hours > 0 ?
                                $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Minutes > 0 ?
                                $"{(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                $"{(DateTime.Now - createdAt).Seconds} secs",
                            Section = await _context.OrderTableMappings
                                .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                .Select(otm => otm.Table.Section.Name)
                                .FirstOrDefaultAsync(),
                            Table = string.Join(", ", await _context.OrderTableMappings
                                    .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                    .Select(otm => otm.Table.Name)
                                    .ToListAsync()),
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
                                    OrderItemId = orderItem.Id,
                                    Id = item.Id,
                                    ItemName = item.Name,
                                    ItemQuantity = (int)(orderItem.Quantity - (orderItem.ReadyItemsCount ?? 0)),
                                    ItemInstruction = orderItem.Comment,
                                    Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                        .Select(om => new ModifierDetails
                                        {
                                            ModifierId = om.ModifierId,
                                            ModifierName = om.Modifier.Name,
                                        })
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the KOT by category");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while fetching the KOT by category", ex);
            }
        }
        public async Task<KotViewModel> GetMarkedAsPreparedModalAsync(int pageIndex, int orderId, int categoryId, bool inReady)
        {
            try
            {
                KotViewModel kotViewModel = new();
                if (inReady)
                {
                    kotViewModel = await GetReadyItemsAsync(categoryId, pageIndex, orderId);
                }
                else
                {
                    kotViewModel = await GetKotByCategoryAsync(categoryId, pageIndex, 4, orderId);
                }
                return kotViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the marked as prepared modal");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while fetching the marked as prepared modal", ex);
            }
        }
        public async Task<KotViewModel> MarkItemsAsReadyAsync(int pageIndex, List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string readyItemsJson = JsonSerializer.Serialize(readyItems);
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL mark_items_as_ready({0}, {1}::jsonb, {2})",
                    orderId,
                    readyItemsJson,
                    userId
                );
                await transaction.CommitAsync();
                await _hubContext.Clients.All.SendAsync("UpdateKOT", orderId);
                _logger.LogInformation("Items marked as ready successfully");
                KotViewModel kotViewModel = await GetKotByCategoryAsync(categoryId, pageIndex, 4);
                return kotViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking items as ready");
                Console.WriteLine(ex.Message);
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while marking items as ready", ex);

            }
        }
        public async Task<KotViewModel> MarkItemsAsInPreparedAsync(int pageIndex, List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string readyItemsJson = JsonSerializer.Serialize(readyItems);
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL mark_items_as_in_prepared({0}, {1}::jsonb, {2})",
                    orderId,
                    readyItemsJson,
                    userId
                );
                await transaction.CommitAsync();
                KotViewModel kotViewModel = await GetReadyItemsAsync(categoryId, pageIndex);
                await _hubContext.Clients.All.SendAsync("UpdateKOT", orderId);
                _logger.LogInformation("Items marked as in prepared successfully");
                return kotViewModel;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while marking items as in prepared");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while marking items as in prepared", ex);
            }
        }
        public async Task<KotViewModel> GetReadyItemsAsync(int categoryId, int pageIndex = 1, int? orderId = null)
        {
            try
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
                    orders = await _context.Orders.Where(o => o.IsDeleted == false && o.Status != "Completed" && o.Status != "Served").ToListAsync();
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
                            OrderDuration = (DateTime.Now - createdAt).Days > 0 ?
                                $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Hours > 0 ?
                                $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                (DateTime.Now - createdAt).Minutes > 0 ?
                                $"{(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                                $"{(DateTime.Now - createdAt).Seconds} secs",
                            Section = await _context.OrderTableMappings
                                .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                .Select(otm => otm.Table.Section.Name)
                                .FirstOrDefaultAsync(),
                            Table = string.Join(", ", await _context.OrderTableMappings
                                    .Where(otm => otm.OrderId == order.Id && otm.IsDeleted == false)
                                    .Select(otm => otm.Table.Name)
                                    .ToListAsync()),
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
                                    OrderItemId = orderItem.Id,
                                    Id = item.Id,
                                    ItemName = item.Name,
                                    ItemQuantity = orderItem.ReadyItemsCount ?? 0,
                                    ItemInstruction = orderItem.Comment,
                                    Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                        .Select(om => new ModifierDetails
                                        {
                                            ModifierId = om.ModifierId,
                                            ModifierName = om.Modifier.Name,
                                        })
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
                if (pageIndex > TotalPages && TotalPages != 0 && orderId == null)
                {
                    pageIndex = TotalPages;
                }
                if (orderId != null)
                {
                    kotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).ToList();
                }
                else
                {
                    kotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).Skip((pageIndex - 1) * 4).Take(4).ToList();
                }
                KotViewModel kotViewModel = new()
                {
                    Categories = categories.OrderBy(c => c.Id).ToList(),
                    KotOrderCards = kotOrderCards,
                    PageSize = 4,
                    PageIndex = pageIndex,
                    TotalPages = (int)Math.Ceiling((double)kotOrderCards.Count / 4),
                };
                return kotViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the ready items");
                Console.WriteLine(ex.Message);
                throw new Exception("An error occurred while fetching the ready items", ex);
            }
        }
    }
}