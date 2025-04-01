using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public WaitingListViewModel GetWaitingListViewModel()
        {
            List<SectionAndNumberOfWaitingList> sectionAndNumberOfWaitingLists = new List<SectionAndNumberOfWaitingList>();
            int totalCountOfWaitingList = _context.WaitingLists.Where(w => w.IsDeleted == false).Count();
            SectionAndNumberOfWaitingList sectionAndNumberOfWaitingList = new SectionAndNumberOfWaitingList
            {
                SectionId = 0,
                SectionName = "All",
                NumberOfWaitingList = totalCountOfWaitingList
            };
            sectionAndNumberOfWaitingLists.Add(sectionAndNumberOfWaitingList);
            List<Section> sections = _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToList();
            foreach (var section in sections)
            {
                int numberOfWaitingList = _context.WaitingLists.Where(w => w.SectionId == section.Id && w.IsDeleted == false).Count();  
                sectionAndNumberOfWaitingList = new SectionAndNumberOfWaitingList
                {
                    SectionId = section.Id,
                    SectionName = section.Name,
                    NumberOfWaitingList = numberOfWaitingList
                };
                sectionAndNumberOfWaitingLists.Add(sectionAndNumberOfWaitingList);
            }
            List<WaitingListTable> waitingList = _context.WaitingLists
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
                }).ToList();
            foreach (WaitingListTable waiting in waitingList)
            {
                waiting.WaitingTime = waiting.WaitingTime.Substring(0, waiting.WaitingTime.Length - 3) + " hrs " + waiting.WaitingTime.Substring(waiting.WaitingTime.Length - 2) + " mins";
            }
            List<Section> sections2 = _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToList();
            WaitingListViewModel waitingListViewModel = new WaitingListViewModel
            {
                SectionAndNumberOfWaitingLists = sectionAndNumberOfWaitingLists,
                WaitingList = waitingList,
                Sections = sections2
            };
            return waitingListViewModel;
        }

        public IActionResult DeleteWaitingList(int id, int userId)
        {
            WaitingList waitingList = _context.WaitingLists.Find(id);
            if (waitingList == null)
            {
                return new JsonResult(new { success = false, message = "Waiting list not found"});
            }
            waitingList.IsDeleted = true;
            waitingList.UpdatedBy = userId;
            waitingList.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Waiting list deleted successfully"});
        }

        public WaitingListViewModel GetWaitingListDetails(int id)
        {
            WaitingList waitingList = _context.WaitingLists.Find(id);
            if (waitingList == null)
            {
                return new WaitingListViewModel();
            }
            Customer customer = _context.Customers.Find(waitingList.CustomerId);
            WaitingListModal waitingListModal = new WaitingListModal
            {
                Id = waitingList.Id,
                Name = customer.Name ,
                Email = customer.Email,
                MobileNumber = customer.Phone,
                NumberOfPeople = waitingList.NoOfPersons,
                SectionId = (int)waitingList.SectionId
            };
            WaitingListViewModel waitingListViewModel = new WaitingListViewModel
            {
                waitingListModal = waitingListModal
            };
            List<Section> sections = _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToList();
            waitingListViewModel.Sections = sections;
            return waitingListViewModel;
        }

        public IActionResult GetCustomerSuggestions(string email)
        {
            List<Customer> customers = _context.Customers.Where(c => c.Email.Contains(email)).ToList();
            customers.RemoveAll(c => _context.WaitingLists.Any(w => w.CustomerId == c.Id && w.IsDeleted == false));
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

        public WaitingListViewModel GetWaitingListBasedOnSection(int sectionId)
        {
            List<WaitingListTable> waitingList = new List<WaitingListTable>();
            if (sectionId == 0)
            {
                waitingList = _context.WaitingLists
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
                    }).ToList();
            }
            else
            {
                waitingList = _context.WaitingLists
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
                    }).ToList();
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

        public JsonResult GetAvailableTables(int sectionId)
        {
            List<Table> availableTables = _context.Tables
                .Where(t => t.SectionId == sectionId && t.IsDeleted == false && t.Status == "Available")
                .OrderBy(t => t.Id)
                .Select(t => new Table
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList();
            return new JsonResult(availableTables);
        }

        public IActionResult AssignTable(int waitingListId, int tableId, int userId, int sectionId)
        {
            WaitingList waitingList = _context.WaitingLists.Find(waitingListId);
            Table table = _context.Tables.Find(tableId);
            table.Status = "Assigned";
            waitingList.IsDeleted = true;
            waitingList.SectionId = sectionId;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;
            _context.SaveChanges();
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
            _context.Orders.Add(order);
            _context.SaveChanges();
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
            _context.OrderTableMappings.Add(orderTableMapping);
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Table assigned successfully"});
        }
    }
}