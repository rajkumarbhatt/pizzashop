using System.Data;
using System.Text.Json;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BLL.Services
{
    public class WaitingListService : IWaitingListService
    {
        private readonly PizzaShopContext _context;
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<WaitingListService> _logger;
        public WaitingListService(PizzaShopContext context, IOrderAppService orderAppService, ILogger<WaitingListService> logger)
        {
            _logger = logger;
            _context = context;
            _orderAppService = orderAppService;
        }
        public async Task<WaitingListViewModel> GetWaitingListViewModelAsync()
        {
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT get_waiting_list_data()";
                    command.CommandType = CommandType.Text;

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var jsonString = reader.GetString(0);

                            var waitingListViewModel = JsonSerializer.Deserialize<WaitingListViewModel>(
                                jsonString,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                });

                            return waitingListViewModel;
                        }
                    }
                }
                return new WaitingListViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the waiting list view model");
                Console.WriteLine(ex.Message);
                return new WaitingListViewModel();
            }
        }
        public async Task<IActionResult> DeleteWaitingListAsync(int id, int userId)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL delete_waiting_list_entry({0}, {1})", id, userId);
                _logger.LogInformation("Waiting list with ID {Id} deleted successfully by user {UserId}", id, userId);
                return new JsonResult(new { success = true, message = "Waiting list deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the waiting list with ID {Id}", id);
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = "An error occurred while deleting the waiting list" });
            }
        }
        public async Task<WaitingListViewModel> GetWaitingListDetailsAsync(int id)
        {
            try
            {
                using var conn = (Npgsql.NpgsqlConnection)_context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = new Npgsql.NpgsqlCommand("SELECT get_waiting_list_details(@p_id)", conn);
                cmd.Parameters.AddWithValue("p_id", id);

                var jsonResult = (string)await cmd.ExecuteScalarAsync();

                await conn.CloseAsync();

                var waitingListViewModel = JsonSerializer.Deserialize<WaitingListViewModel>(jsonResult);
                return waitingListViewModel;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the waiting list details for ID {Id}", id);
                Console.WriteLine(ex.Message);
                return new WaitingListViewModel();
            }
        }
        public async Task<IActionResult> GetCustomerSuggestionsAsync(string email)
        {
            try
            {
                using var conn = (Npgsql.NpgsqlConnection)_context.Database.GetDbConnection();
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT get_customer_suggestions(@email)", conn);
                cmd.Parameters.AddWithValue("email", email);

                var result = await cmd.ExecuteScalarAsync();
                var json = result?.ToString() ?? "[]";

                var suggestions = JsonSerializer.Deserialize<List<CustomerDetailsSuggestions>>(json);

                return new JsonResult(new { success = true, customerSuggetions = suggestions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching customer suggestions for email {Email}", email);
                return new JsonResult(new { success = false, message = "An error occurred while fetching customer suggestions" });
            }
        }
        public async Task<WaitingListViewModel> GetWaitingListBasedOnSectionAsync(int sectionId)
        {
            try
            {
                using var conn = (Npgsql.NpgsqlConnection)_context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = new Npgsql.NpgsqlCommand("SELECT get_waiting_list_by_section(@p_section_id)", conn);
                cmd.Parameters.AddWithValue("p_section_id", sectionId);
                var jsonResult = (string)await cmd.ExecuteScalarAsync();

                await conn.CloseAsync();

                var waitingListViewModel = JsonSerializer.Deserialize<WaitingListViewModel>(jsonResult);

                return waitingListViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the waiting list based on section ID {SectionId}", sectionId);
                Console.WriteLine(ex.Message);
                return new WaitingListViewModel();
            }
        }
        public async Task<JsonResult> GetAvailableTablesAsync(int sectionId)
        {
            try
            {
                using var conn = (Npgsql.NpgsqlConnection)_context.Database.GetDbConnection();
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT get_available_tables(@section_id)", conn);
                cmd.Parameters.AddWithValue("section_id", sectionId);

                var result = await cmd.ExecuteScalarAsync();
                var json = string.IsNullOrWhiteSpace(result?.ToString()) ? "[]" : result.ToString();

                var availableTables = JsonSerializer.Deserialize<List<Table>>(json);
                return new JsonResult(availableTables);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching available tables for section ID {SectionId}", sectionId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new List<Table>());
            }
        }
        public async Task<IActionResult> AssignTableAsync(int waitingListId, List<int> tableIds, int userId, int sectionId)
        {
            try
            {
                WaitingList waitingList = await _context.WaitingLists.FindAsync(waitingListId) ?? new WaitingList();
                Customer customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == waitingList.CustomerId) ?? new Customer();
                WaitingListModal waitingListModal = new WaitingListModal
                {
                    Id = waitingList.Id,
                    Name = customer.Name,
                    Email = customer.Email ?? "",
                    MobileNumber = customer.Phone ?? "",
                    NumberOfPeople = waitingList.NoOfPersons,
                    SectionId = sectionId
                };
                return await _orderAppService.AssignTablesToCustomerAsync(waitingListModal, tableIds, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning tables to waiting list ID {WaitingListId}", waitingListId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = "An error occurred while assigning tables" });
            }
        }
    }
}