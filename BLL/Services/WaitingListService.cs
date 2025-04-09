using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class WaitingListService : IWaitingListService
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;
        public WaitingListService(PizzaShopContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        public async Task<WaitingListViewModel> GetWaitingListViewModelAsync()
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

        public async Task<IActionResult> DeleteWaitingListAsync(int id, int userId)
        {
            WaitingList waitingList = await _context.WaitingLists.FindAsync(id);
            if (waitingList == null)
            {
                return new JsonResult(new { success = false, message = "Waiting list not found" });
            }
            waitingList.IsDeleted = true;
            waitingList.UpdatedBy = userId;
            waitingList.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Waiting list deleted successfully" });
        }

        public async Task<WaitingListViewModel> GetWaitingListDetailsAsync(int id)
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

        public async Task<IActionResult> GetCustomerSuggestionsAsync(string email)
        {
            List<Customer> customers = await _context.Customers.Where(c => c.Email.Contains(email)).ToListAsync();
            customers.RemoveAll(c => _context.WaitingLists.Any(w => w.CustomerId == c.Id && w.IsDeleted == false));
            customers.RemoveAll(c => _context.Orders.Any(o => o.CustomerId == c.Id && o.Status != "Completed"));
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

        public async Task<WaitingListViewModel> GetWaitingListBasedOnSectionAsync(int sectionId)
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

        public async Task<JsonResult> GetAvailableTablesAsync(int sectionId)
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

        public async Task<IActionResult> AssignTableAsync(int waitingListId, int tableId, int userId, int sectionId)
        {
            WaitingList waitingList = await _context.WaitingLists.FindAsync(waitingListId);
            if (waitingList == null)
            {
                return new JsonResult(new { success = false, message = "Waiting list not found" });
            }

            Customer customer = await _context.Customers.FindAsync(waitingList.CustomerId);
            if (customer == null)
            {
                return new JsonResult(new { success = false, message = "Customer not found" });
            }

            if (await _context.Orders.AnyAsync(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has an ongoing order" });
            }

            Table table = await _context.Tables.FindAsync(tableId);
            if (table == null)
            {
                return new JsonResult(new { success = false, message = "Table not found" });
            }
            else if (table.Capacity < waitingList.NoOfPersons)
            {
                return new JsonResult(new { success = false, message = "Customers can't be managed in selected table" });
            }
            else if (table.Status != "Available")
            {
                return new JsonResult(new { success = false, message = "Table is not available" });
            }

            table.Status = "Assigned";
            waitingList.IsDeleted = true;
            waitingList.SectionId = sectionId;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            Order order = new Order
            {
                TotalAmount = 0,
                Status = "Pending",
                PaymentMode = "Cash",
                IsDeleted = false,
                CustomerId = waitingList.CustomerId,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderTableMapping orderTableMapping = new OrderTableMapping
            {
                OrderId = order.Id,
                TableId = tableId,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId,
                IsDeleted = false
            };

            await _context.OrderTableMappings.AddAsync(orderTableMapping);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Table assigned successfully" });
        }
    }
}