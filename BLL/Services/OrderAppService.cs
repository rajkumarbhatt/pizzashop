using System.Text.Json;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
namespace BLL.Services;

public class OrderAppService : IOrderAppService
{
    private readonly PizzaShopContext _context;
    private readonly ILogger<OrderAppService> _logger;
    public OrderAppService(PizzaShopContext context, ILogger<OrderAppService> logger)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<OrderAppViewModel> GetOrderAppViewModelAsync()
    {
        try
        {
            List<Section> sections = await _context.Sections
            .Where(s => s.IsDeleted == false)
            .OrderBy(s => s.Id)
            .ToListAsync();

            List<AccordianItem> accordianItems = new List<AccordianItem>();

            foreach (Section section in sections)
            {
                List<Table> tables = await _context.Tables
                    .Where(t => t.SectionId == section.Id && t.IsDeleted == false)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

                List<TableCard> tableCards = new List<TableCard>();

                foreach (var table in tables)
                {
                    var orderTableMapping = await _context.OrderTableMappings
                        .FirstOrDefaultAsync(otm => otm.TableId == table.Id && otm.IsDeleted == false);

                    var createdAt = orderTableMapping?.CreatedAt ?? DateTime.Now;

                    string currentOrderTime = (DateTime.Now - createdAt).Days > 0 ?
                        $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                        (DateTime.Now - createdAt).Hours > 0 ?
                        $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                        (DateTime.Now - createdAt).Minutes > 0 ?
                        $"{(DateTime.Now - createdAt).Minutes} mins {(DateTime.Now - createdAt).Seconds} secs" :
                        $"{(DateTime.Now - createdAt).Seconds} secs";

                    tableCards.Add(new TableCard
                    {
                        OrderId = orderTableMapping?.OrderId ?? 0,
                        TableId = table.Id,
                        OrderTotal = orderTableMapping != null ?
                            (await _context.Orders.FindAsync(orderTableMapping.OrderId))?.TotalAmount :
                            0,
                        TableName = table.Name,
                        TableStatus = table.Status,
                        TableCapacity = table.Status == "Available" ?
                            table.Capacity :
                            orderTableMapping?.NoOfPersons ?? 0,
                        CurentOrderTime = orderTableMapping != null ? currentOrderTime : "N/A"
                    });
                }

                AccordianItem accordianItem = new AccordianItem
                {
                    SectionId = section.Id,
                    SectionName = section.Name,
                    NumberOfAvailableTables = tables.Count(t => t.Status == "Available"),
                    NumberOfAssignedTables = tables.Count(t => t.Status == "Assigned"),
                    NumberOfRunningTables = tables.Count(t => t.Status == "Running"),
                    NumberOfSelectedTables = 0,
                    TableCards = tableCards
                };

                accordianItems.Add(accordianItem);
            }

            OrderAppViewModel orderAppViewModel = new OrderAppViewModel
            {
                Sections = accordianItems
            };

            return orderAppViewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the order app view model");
            Console.WriteLine(ex.Message);
            return new OrderAppViewModel
            {
                Sections = new List<AccordianItem>()
            };
        }
    }
    public async Task<IActionResult> AddToWaitingListAsync(WaitingListModal waitingListModal, int userId)
    {
        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (waitingListModal.Id == -1)
                    {
                        string name = waitingListModal.Name;
                        string email = waitingListModal.Email;
                        string mobileNumber = waitingListModal.MobileNumber;
                        string numberOfPeople = waitingListModal.NumberOfPeople.ToString();
                        string sectionId = waitingListModal.SectionId.ToString();

                        if (await _context.WaitingLists.AnyAsync(w => w.Customer.Email == email && w.IsDeleted == false))
                        {
                            _logger.LogWarning("Customer with email {Email} who is already in waiting list wad added again to waiting list by user {UserId}", email, userId);
                            return new JsonResult(new
                            {
                                success = false,
                                message = "Customer already in waiting list"
                            });
                        }

                        Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
                        if (customer != null && await _context.Orders.AnyAsync(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
                        {
                            _logger.LogWarning("Customer with email {Email} who is already having an ongoing order wad added again to waiting list by user {UserId}", email, userId);
                            return new JsonResult(new
                            {
                                success = false,
                                message = "Customer already has an ongoing order"
                            });
                        }

                        await _context.Database.ExecuteSqlRawAsync(
                            "CALL add_to_waiting_list({0}, {1}, {2}, {3}, {4}, {5})",
                            waitingListModal.Name,
                            waitingListModal.Email,
                            waitingListModal.MobileNumber,
                            waitingListModal.SectionId,
                            waitingListModal.NumberOfPeople,
                            userId
                        );
                        await transaction.CommitAsync();
                        _logger.LogInformation("Customer with email {Email} added to waiting list by user {UserId}", email, userId);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Added to waiting list successfully"
                        });
                    }
                    else
                    {
                        WaitingList waitingList = await _context.WaitingLists.FindAsync(waitingListModal.Id);
                        if (waitingList == null)
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = "Waiting list not found"
                            });
                        }

                        await _context.Database.ExecuteSqlRawAsync(
                            "CALL update_customer_and_waiting_list({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                            waitingListModal.Id,
                            waitingListModal.Name,
                            waitingListModal.Email,
                            waitingListModal.MobileNumber,
                            waitingListModal.SectionId,
                            waitingListModal.NumberOfPeople,
                            userId
                        );
                        await transaction.CommitAsync();
                        _logger.LogInformation("Waiting list updated successfully by user {UserId}", userId);
                        return new JsonResult(new
                        {
                            success = true,
                            message = "Waiting list updated successfully"
                        });
                    }
                }
                catch
                {
                    _logger.LogError("An error occurred while adding to waiting list by user {UserId}", userId);
                    await transaction.RollbackAsync();
                    return new JsonResult(new
                    {
                        success = false,
                        message = "An error occurred while processing the request"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding to waiting list by user {UserId}", userId);
            Console.WriteLine(ex.Message);
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while processing the request"
            });
        }
    }
    public async Task<JsonResult> GetWaitingListForCurrentSectionAsync(int sectionId)
    {
        try
        {
            List<WaitingList> waitingLists = await _context.WaitingLists
            .Where(w => w.SectionId == sectionId && w.IsDeleted == false)
            .ToListAsync();

            List<WaitingListTable> waitingListTables = new List<WaitingListTable>();

            foreach (WaitingList waitingList in waitingLists)
            {
                Customer customer = await _context.Customers.FindAsync(waitingList.CustomerId);
                WaitingListTable waitingListTable = new WaitingListTable
                {
                    TokenNumber = waitingList.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    PhoneNumber = customer.Phone,
                    NumberOfPersons = waitingList.NoOfPersons
                };
                waitingListTables.Add(waitingListTable);
            }

            return new JsonResult(new
            {
                success = true,
                customerDetails = waitingListTables
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving waiting list for section {SectionId}", sectionId);
            Console.WriteLine(ex.Message);
            return new JsonResult(new
            {
                success = false,
                message = "An error occurred while processing the request"
            });
        }
    }
    public async Task<IActionResult > AssignTablesToCustomerAsync(WaitingListModal modal, List<int> tableIds, int userId)
    {
        try
        {
            using var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand("SELECT assign_tables_to_customer(@id, @name, @email, @mobile, @section_id, @num_people, @table_ids, @user_id)", conn);
            cmd.Parameters.AddWithValue("id", modal.Id);
            cmd.Parameters.AddWithValue("name", modal.Name);
            cmd.Parameters.AddWithValue("email", modal.Email);
            cmd.Parameters.AddWithValue("mobile", modal.MobileNumber);
            cmd.Parameters.AddWithValue("section_id", modal.SectionId);
            cmd.Parameters.AddWithValue("num_people", modal.NumberOfPeople);
            cmd.Parameters.AddWithValue("table_ids", tableIds);
            cmd.Parameters.AddWithValue("user_id", userId);

            var result = await cmd.ExecuteScalarAsync();
            return new JsonResult(JsonSerializer.Deserialize<object>(result.ToString() ?? "{}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign tables via SP");
            return new JsonResult(new { success = false, message = "Failed to assign tables" });
        }
    }
}