using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class WaitingListService : IWaitingListService
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;
        private readonly IOrderAppService _orderAppService;
        private readonly ILogger<WaitingListService> _logger;
        public WaitingListService(PizzaShopContext context, IJwtService jwtService, IOrderAppService orderAppService, ILogger<WaitingListService> logger)
        {
            _logger = logger;
            _context = context;
            _jwtService = jwtService;
            _orderAppService = orderAppService;
        }
        public async Task<WaitingListViewModel> GetWaitingListViewModelAsync()
        {
            try
            {
                List<SectionAndNumberOfWaitingList> sectionAndNumberOfWaitingLists = new List<SectionAndNumberOfWaitingList>();
                int totalCountOfWaitingList = await _context.WaitingLists.Where(w => w.IsDeleted == false).CountAsync();
                SectionAndNumberOfWaitingList sectionAndNumberOfWaitingList = new SectionAndNumberOfWaitingList
                {
                    SectionId = 0,
                    SectionName = "All",
                    NumberOfWaitingList = totalCountOfWaitingList
                };
                sectionAndNumberOfWaitingLists.Add(sectionAndNumberOfWaitingList);
                List<Section> sections = await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync();
                foreach (var section in sections)
                {
                    int numberOfWaitingList = await _context.WaitingLists.Where(w => w.SectionId == section.Id && w.IsDeleted == false).CountAsync();
                    sectionAndNumberOfWaitingList = new SectionAndNumberOfWaitingList
                    {
                        SectionId = section.Id,
                        SectionName = section.Name,
                        NumberOfWaitingList = numberOfWaitingList
                    };
                    sectionAndNumberOfWaitingLists.Add(sectionAndNumberOfWaitingList);
                }
                List<WaitingListTable> waitingList = await _context.WaitingLists
                .Where(w => w.IsDeleted == false)
                .Select(w => new WaitingListTable
                {
                    TokenNumber = w.Id,
                    CreatedAt = w.CreatedAt.HasValue ? w.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                    PhoneNumber = w.Customer.Phone,
                    WaitingTime = w.CreatedAt.HasValue ? DateTime.Now.Subtract(w.CreatedAt.Value).ToString(@"hh\:mm") : "N/A",
                    Name = w.Customer.Name,
                    NumberOfPersons = w.NoOfPersons,
                    Email = w.Customer.Email
                }).ToListAsync();
                foreach (WaitingListTable waiting in waitingList)
                {
                    waiting.WaitingTime = waiting.WaitingTime.Substring(0, waiting.WaitingTime.Length - 3) + " hrs " + waiting.WaitingTime.Substring(waiting.WaitingTime.Length - 2) + " mins";
                }
                List<Section> sections2 = await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync();
                WaitingListViewModel waitingListViewModel = new WaitingListViewModel
                {
                    SectionAndNumberOfWaitingLists = sectionAndNumberOfWaitingLists,
                    WaitingList = waitingList,
                    Sections = sections2
                };
                return waitingListViewModel;
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
                WaitingList waitingList = await _context.WaitingLists.FindAsync(id);
                if (waitingList == null)
                {
                    return new WaitingListViewModel();
                }
                Customer customer = await _context.Customers.FindAsync(waitingList.CustomerId);
                WaitingListModal waitingListModal = new WaitingListModal
                {
                    Id = waitingList.Id,
                    Name = customer.Name,
                    Email = customer.Email,
                    MobileNumber = customer.Phone,
                    NumberOfPeople = waitingList.NoOfPersons,
                    SectionId = (int)waitingList.SectionId
                };
                WaitingListViewModel waitingListViewModel = new WaitingListViewModel
                {
                    waitingListModal = waitingListModal
                };
                List<Section> sections = await _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToListAsync();
                waitingListViewModel.Sections = sections;
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
                List<Customer> customers = await _context.Customers.Where(c => c.Email.ToLower().Contains(email.ToLower())).ToListAsync();
                customers.RemoveAll(c => _context.WaitingLists.Any(w => w.CustomerId == c.Id && w.IsDeleted == false));
                customers.RemoveAll(c => _context.Orders.Any(o => o.CustomerId == c.Id && o.Status != "Completed" && o.Status != "Cancelled"));
                List<CustomerDetailsSuggestions> customerSuggetions = new List<CustomerDetailsSuggestions>();
                foreach (Customer customer in customers)
                {
                    CustomerDetailsSuggestions customerDetailsSuggestions = new CustomerDetailsSuggestions
                    {
                        Name = customer.Name,
                        Email = customer.Email,
                        MobileNumber = customer.Phone
                    };
                    customerSuggetions.Add(customerDetailsSuggestions);
                }
                return new JsonResult(new { success = true, customerSuggetions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customer suggestions for email {Email}", email);
                Console.WriteLine(ex.Message);
                return new JsonResult(new { success = false, message = "An error occurred while fetching customer suggestions" });
            }
        }
        public async Task<WaitingListViewModel> GetWaitingListBasedOnSectionAsync(int sectionId)
        {
            try
            {
                List<WaitingListTable> waitingList = new List<WaitingListTable>();
                if (sectionId == 0)
                {
                    waitingList = await _context.WaitingLists
                        .Where(w => w.IsDeleted == false)
                        .Select(w => new WaitingListTable
                        {
                            TokenNumber = w.Id,
                            CreatedAt = w.CreatedAt.HasValue ? w.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                            PhoneNumber = w.Customer.Phone,
                            WaitingTime = w.CreatedAt.HasValue ? DateTime.Now.Subtract(w.CreatedAt.Value).ToString(@"hh\:mm") : "N/A",
                            Name = w.Customer.Name,
                            NumberOfPersons = w.NoOfPersons,
                            Email = w.Customer.Email
                        }).ToListAsync();
                }
                else
                {
                    waitingList = await _context.WaitingLists
                        .Where(w => w.SectionId == sectionId && w.IsDeleted == false)
                        .Select(w => new WaitingListTable
                        {
                            TokenNumber = w.Id,
                            CreatedAt = w.CreatedAt.HasValue ? w.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                            PhoneNumber = w.Customer.Phone,
                            WaitingTime = w.CreatedAt.HasValue ? DateTime.Now.Subtract(w.CreatedAt.Value).ToString(@"hh\:mm") : "N/A",
                            Name = w.Customer.Name,
                            NumberOfPersons = w.NoOfPersons,
                            Email = w.Customer.Email
                        }).ToListAsync();
                }
                foreach (WaitingListTable waiting in waitingList)
                {
                    waiting.WaitingTime = waiting.WaitingTime.Substring(0, waiting.WaitingTime.Length - 3) + " hrs " + waiting.WaitingTime.Substring(waiting.WaitingTime.Length - 2) + " mins";
                }
                WaitingListViewModel waitingListViewModel = new WaitingListViewModel
                {
                    WaitingList = waitingList
                };
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
                List<Table> availableTables = await _context.Tables
            .Where(t => t.SectionId == sectionId && t.IsDeleted == false && t.Status == "Available")
            .OrderBy(t => t.Id)
            .Select(t => new Table
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
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