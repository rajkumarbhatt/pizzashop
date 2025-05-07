using BLL.Interfaces;
using DAL.DBContext;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly PizzaShopContext _context;
    public DashboardService(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<DashboardViewModel> GetDashboardDataAsync(string? TimePeriod = "Current Month", string? fromDate2 = null, string? toDate2 = null)
    {
        DateTime fromDate = DateTime.Now;
        DateTime toDate = DateTime.Now;
        if (TimePeriod == "Current Month")
        {
            fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            toDate = fromDate.AddMonths(1).AddDays(-1);
        }
        int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        if (TimePeriod == "Last 7 Days")
        {
            fromDate = DateTime.Now.AddDays(-7);
            toDate = DateTime.Now;
        }
        else if (TimePeriod == "This Year")
        {
            fromDate = new DateTime(DateTime.Now.Year, 1, 1);
            toDate = new DateTime(DateTime.Now.Year, 12, 31);
        }
        else if (TimePeriod == "Today")
        {
            fromDate = DateTime.Now.Date;
            toDate = fromDate.AddDays(1);
        }
        else if (TimePeriod == "Last 30 Days")
        {
            fromDate = DateTime.Now.AddDays(-30);
            toDate = DateTime.Now;
        }
        else if (TimePeriod == "Custom Date")
        {
            if (fromDate2 != null && toDate2 != null)
            {
                fromDate = DateTime.Parse(fromDate2);
                toDate = DateTime.Parse(toDate2);
            }
            if (fromDate == toDate)
            {
                toDate = fromDate.AddDays(1);
            }
        }

        DashboardViewModel dashboardData = new DashboardViewModel
        {
            TotalOrders = await _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled").CountAsync(),
            TotalSales = Math.Round((double)await _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled").SumAsync(o => o.TotalAmount), 2),
            AverageOrderValue = Math.Round((double)await _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled").AverageAsync(o => o.TotalAmount), 2),
            AverageWaitingTime = Math.Round((double)(await _context.Orders
                        .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled")
                        .ToListAsync())
                    .Where(o => o.UpdatedAt != null)
                    .Average(o => (o.UpdatedAt - o.CreatedAt)?.TotalMinutes ?? 0), 2),
            WaitingListCount = await _context.WaitingLists.CountAsync(w => w.CreatedAt >= fromDate && w.CreatedAt <= toDate && w.IsDeleted == false),
            NewCustomerCount = await _context.Customers.CountAsync(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
            TopSellingItems = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate && oi.Order.Status != "Cancelled")
                .GroupBy(oi => new
                {
                    oi.ItemId,
                    oi.Item.Name,
                    oi.Item.ImageUrl
                })
                .Select(g => new DashboardItems
                {
                    Id = g.Key.ItemId,
                    Name = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    NumberOfOrders = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(g => g.NumberOfOrders)
                .Take(2)
                .ToListAsync(),
            LeastSellingItems = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate && oi.Order.Status != "Cancelled")
                .GroupBy(oi => new
                {
                    oi.ItemId,
                    oi.Item.Name,
                    oi.Item.ImageUrl
                })
                .Select(g => new DashboardItems
                {
                    Id = g.Key.ItemId,
                    Name = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    NumberOfOrders = g.Sum(oi => oi.Quantity)
                })
                .OrderBy(g => g.NumberOfOrders)
                .Take(2)
                .ToListAsync(),
            RevenueData = new RevenueData
            {
                Labels = TimePeriod
                    switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth)
                        .Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString("dd MMM"))
                        .ToList(),
                    "Last 7 Days" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day).ToString("dd MMM"))
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM"))
                    .ToList(),
                    "Last 30 Days" => Enumerable.Range(0, 30)
                    .Select(day => fromDate.AddDays(day).ToString("dd MMM"))
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => $"{fromDate.AddHours(8 + hour):hh tt} - {fromDate.AddHours(8 + hour + 1):hh tt}")
                    .ToList(),
                    "Custom Date" => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(offset => fromDate.AddDays(offset).ToString("dd MMM"))
                    .ToList(),
                    _ => new List<string>(),
                },
                Values = TimePeriod
                    switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth)
                        .Select(day => fromDate.AddDays(day - 1))
                        .GroupJoin(
                            _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                            date => date.Date,
                            order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Date : DateTime.MinValue.Date,
                            (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                        )
                        .ToList(),
                    "Last 7 Days" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                        date => date.Date,
                        order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                    )
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1))
                    .GroupJoin(
                        _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                        date => date.Month,
                        order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Month : 0,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                    )
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => fromDate.AddHours(8 + hour))
                    .GroupJoin(
                        _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                        date => date.Hour,
                        order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Hour : 0,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                    )
                    .ToList(),
                    "Last 30 Days" => Enumerable.Range(0, 30)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                        date => date.Date,
                        order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                    )
                    .ToList(),
                    "Custom Date" => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled"),
                        date => date.Date,
                        order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, orders) => (decimal)Math.Round((double)orders.Sum(o => o.TotalAmount), 2)
                    )
                    .ToList(),
                    _ => new List<decimal>()
                }
            },
            CustomerGrowthData = new RevenueData
            {
                Labels = TimePeriod
                    switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth)
                        .Select(day => new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString("dd MMM"))
                        .ToList(),
                    "Last 7 Days" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day).ToString("dd MMM"))
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1).ToString("MMMM"))
                    .ToList(),
                    "Last 30 Days" => Enumerable.Range(0, 30)
                    .Select(day => fromDate.AddDays(day).ToString("dd MMM"))
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => $"{fromDate.AddHours(8 + hour):hh tt} - {fromDate.AddHours(8 + hour + 1):hh tt}")
                    .ToList(),
                    "Custom Date" => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(offset => fromDate.AddDays(offset).ToString("dd MMM"))
                    .ToList(),
                    _ => new List<string>(),
                },
                Values = TimePeriod
                    switch
                {
                    "Current Month" => Enumerable.Range(1, daysInMonth)
                        .Select(day => fromDate.AddDays(day - 1))
                        .GroupJoin(
                            _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                            date => date.Date,
                            customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Date : DateTime.MinValue.Date,
                            (date, customers) => (decimal)customers.Count()
                        )
                        .ToList(),
                    "Last 7 Days" => Enumerable.Range(0, 7)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                        date => date.Date,
                        customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "This Year" => Enumerable.Range(1, 12)
                    .Select(month => new DateTime(DateTime.Now.Year, month, 1))
                    .GroupJoin(
                        _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                        date => date.Month,
                        customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Month : 0,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "Today" => Enumerable.Range(0, 12)
                    .Select(hour => fromDate.AddHours(8 + hour))
                    .GroupJoin(
                        _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                        date => date.Hour,
                        customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Hour : 0,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "Last 30 Days" => Enumerable.Range(0, 30)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                        date => date.Date,
                        customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    "Custom Date" => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(day => fromDate.AddDays(day))
                    .GroupJoin(
                        _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate),
                        date => date.Date,
                        customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Date : DateTime.MinValue.Date,
                        (date, customers) => (decimal)customers.Count()
                    )
                    .ToList(),
                    _ => new List<decimal>()
                }
            }
        };
        return dashboardData;
    }
}