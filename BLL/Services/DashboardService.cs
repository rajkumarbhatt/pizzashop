using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
namespace BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly PizzaShopContext _context;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DashboardService> _logger;
    private readonly IJwtService _jwtService;
    public DashboardService(PizzaShopContext context, ILogger<DashboardService> logger, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _context = context;
    }
    public async Task<DashboardViewModel> GetDashboardDataAsync(string? TimePeriod = "Current Month", string? fromDate2 = null, string? toDate2 = null)
    {
        try
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
            if (TimePeriod == "Custom Date")
            {
                toDate = toDate.AddDays(1).AddTicks(-1);
            }

            List<string> labelsForGraph = (toDate - fromDate).TotalDays switch
            {
                <= 31 => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(day => fromDate.AddDays(day).ToString("dd MMM"))
                    .ToList(),
                > 31 and <= 365 => Enumerable.Range(0, ((toDate.Year - fromDate.Year) * 12 + toDate.Month - fromDate.Month) + 1)
                    .Select(day => fromDate.AddMonths(day).ToString("MMMM yyyy"))
                    .ToList(),
                > 365 => Enumerable.Range(0, toDate.Year - fromDate.Year + 1)
                    .Select(day => new DateTime(fromDate.Year + day, 1, 1).ToString("yyyy"))
                    .ToList(),
                _ => new List<string>(),
            };

            List<Order> orders = await _context.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled").ToListAsync();
            List<Customer> customers = _context.Customers.Where(c => c.CreatedAt >= fromDate && c.CreatedAt <= toDate).ToList();


            DashboardViewModel dashboardData = new DashboardViewModel
            {
                TotalOrders = orders.Count,
                TotalSales = orders.Count() > 0 ? Math.Round((double)orders.Sum(o => o.TotalAmount), 2) : 0,
                AverageOrderValue = orders.Count() > 0 ? Math.Round((double)orders.Average(o => o.TotalAmount), 2) : 0,
                AverageWaitingTime = orders.Count() > 0 ? Math.Round((double)(await _context.Orders
                            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.Status != "Cancelled")
                            .ToListAsync())
                        .Where(o => o.UpdatedAt != null)
                        .Average(o => (o.UpdatedAt - o.CreatedAt)?.TotalMinutes ?? 0), 2) : 0,
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
                    Labels = labelsForGraph,
                    Values = (toDate - fromDate).TotalDays switch
                    {
                        <= 31 => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                            .Select(day => fromDate.AddDays(day))
                            .GroupJoin(
                                orders,
                                date => date.Date,
                                order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Date : DateTime.MinValue.Date,
                                (date, orders) => Convert.ToDecimal(Math.Round((double)orders.Sum(o => o.TotalAmount), 2))
                            )
                            .ToList(),
                        > 31 and <= 365 => Enumerable.Range(0, ((toDate.Year - fromDate.Year) * 12 + toDate.Month - fromDate.Month) + 1)
                            .Select(day => fromDate.AddMonths(day))
                            .GroupJoin(
                                orders,
                                date => date.Month,
                                order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Month : 0,
                                (date, orders) => Convert.ToDecimal(Math.Round((double)orders.Sum(o => o.TotalAmount), 2))
                            )
                            .ToList(),
                        > 365 => Enumerable.Range(0, toDate.Year - fromDate.Year + 1)
                            .Select(day => new DateTime(fromDate.Year + day, 1, 1))
                            .GroupJoin(
                                orders,
                                date => date.Year,
                                order => order.CreatedAt.HasValue ? order.CreatedAt.Value.Year : 0,
                                (date, orders) => Convert.ToDecimal(Math.Round((double)orders.Sum(o => o.TotalAmount), 2))
                            )
                            .ToList(),
                        _ => new List<decimal>(),
                    }
                },
                CustomerGrowthData = new RevenueData
                {
                    Labels = labelsForGraph,
                    Values = (toDate - fromDate).TotalDays switch
                    {
                        <= 31 => Enumerable.Range(0, (toDate - fromDate).Days + 1)
                            .Select(day => fromDate.AddDays(day))
                            .GroupJoin(
                                customers,
                                date => date.Date,
                                customer => customer.CreatedAt.HasValue ? customer.CreatedAt.Value.Date : DateTime.MinValue.Date,
                                (date, customers) => Convert.ToDecimal(customers.Count())
                            )
                            .ToList(),
                        > 31 and <= 365 => Enumerable.Range(0, ((toDate.Year - fromDate.Year) * 12 + toDate.Month - fromDate.Month) + 1)
                            .Select(day => fromDate.AddMonths(day))
                            .GroupJoin(
                                customers,
                                date => date.Month,
                                customer => customer.CreatedAt?.Month ?? 0,
                                (date, customers) => Convert.ToDecimal(customers.Count())
                            )
                            .ToList(),
                        > 365 => Enumerable.Range(0, toDate.Year - fromDate.Year + 1)
                            .Select(day => new DateTime(fromDate.Year + day, 1, 1))
                            .GroupJoin(
                                customers,
                                date => date.Year,
                                customer => customer.CreatedAt?.Year ?? 0,
                                (date, customers) => Convert.ToDecimal(customers.Count())
                            )
                            .ToList(),
                        _ => new List<int>().Select(i => Convert.ToDecimal(i)).ToList(),

                    }
                }
            };
            return dashboardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching dashboard data: {Message}", ex.Message);
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }
    }
    public async Task<IActionResult> EnableTwoFactorAuthenticationAsync()
    {
        string token = _httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? string.Empty;
        int userId = _jwtService.GetUserIdFromJwtTokenAsync(token).Result;
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.TwoFactorEnabled = true;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = userId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _jwtService.SetSessionParametersAsync(userId, user.Username, user.RoleId);
                return new JsonResult(new { success = true, message = "Two-factor authentication enabled successfully." });
            }
            return new JsonResult(new { success = false, message = "User not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while enabling two-factor authentication: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = "An error occurred while enabling two-factor authentication." });
        }
    }
    public async Task<IActionResult> DisableTwoFactorAuthenticationAsync()
    {
        string token = _httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? string.Empty;
        int userId = _jwtService.GetUserIdFromJwtTokenAsync(token).Result;
        try
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.TwoFactorEnabled = false;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = userId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _jwtService.SetSessionParametersAsync(userId, user.Username, user.RoleId);
                return new JsonResult(new { success = true, message = "Two-factor authentication disabled successfully." });
            }
            return new JsonResult(new { success = false, message = "User not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while disabling two-factor authentication: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = "An error occurred while disabling two-factor authentication." });
        }
    }
}