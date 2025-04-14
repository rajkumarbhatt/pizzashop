namespace BLL.Services
{
    using BLL.Interfaces;
    using DAL.DBContext;
    using DAL.Models;
    using DAL.ViewModels;
    using Microsoft.EntityFrameworkCore;

    public class KotService : IKotService
    {
        private readonly PizzaShopContext _context;
        public KotService(PizzaShopContext context)
        {
            _context = context;
        }
        public async Task<KotViewModel> GetKotViewModelAsync()
        {
            List<Category>? categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            List<KotOrderCard> kotOrderCards = new List<KotOrderCard>();
            categories.Add(new Category
            {
                Id = 0,
                Name = "All",
                IsDeleted = false
            });
            List<Order> orders = await _context.Orders.Where(o => o.IsDeleted == false).ToListAsync();
            foreach (var order in orders)
            {
                List<OrderItem> orderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id && oi.IsDeleted == false).ToListAsync();
                if (orderItems.Count > 0)
                {
                    KotOrderCard kotOrderCard = new KotOrderCard
                    {
                        OrderId = order.Id,
                        OrderDuration = "N/A",
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
                                i.Name,
                                i.Price,
                                i.CategoryId
                            })
                            .FirstOrDefaultAsync();

                        if (item != null)
                        {
                            kotOrderCardItems.Add(new KotOrderCardItem
                            {
                                ItemName = item.Name,
                                ItemQuantity = orderItem.Quantity,
                                ItemInstruction= orderItem.Comment,
                                Modifiers = await _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id && om.IsDeleted == false)
                                    .Select(om => om.Modifier.Name)
                                    .ToListAsync(),
                            });
                        }
                    }
                    kotOrderCards.Add(kotOrderCard);
                }
            }
            KotViewModel kotViewModel = new()
            {
                Categories = categories.OrderBy(c => c.Id).ToList(),
                KotOrderCards = kotOrderCards.OrderBy(k => k.OrderId).ToList(),
            };
            return kotViewModel;
        }
    }
}