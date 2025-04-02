using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services;

public class OrderAppService : IOrderAppService
{
    private readonly PizzaShopContext _context;

    public OrderAppService(PizzaShopContext context)
    {
        _context = context;
    }

    public OrderAppViewModel GetOrderAppViewModel()
    {
        List<Section> sections = _context.Sections.Where(s => s.IsDeleted == false).OrderBy(s => s.Id).ToList();
        List<AccordianItem> accordianItems = new List<AccordianItem>();
        foreach (Section section in sections)
        {
            List<Table> tables = _context.Tables.Where(t => t.SectionId == section.Id && t.IsDeleted == false).OrderBy(t => t.Id).ToList();
            AccordianItem accordianItem = new AccordianItem
            {
                SectionId = section.Id,
                SectionName = section.Name,
                NumberOfAvailableTables = section.Tables.Count(t => t.Status == "Available"),
                NumberOfAssignedTables = section.Tables.Count(t => t.Status == "Assigned"),
                NumberOfRunningTables = section.Tables.Count(t => t.Status == "Running"),
                NumberOfSelectedTables = 0,
                TableCards = tables.Select(table => new TableCard
                {
                    TableId = table.Id,
                    TableName = table.Name,
                    TableStatus = table.Status,
                    TableCapacity = table.Status == "Available" ? table.Capacity : (_context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id)?.NoOfPersons ?? 0),
                    CurentOrderTime = _context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id) != null
                    ? ((DateTime.Now - (_context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id)?.CreatedAt ?? DateTime.Now)).Hours > 0
                        ? $"{(DateTime.Now - (_context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id)?.CreatedAt ?? DateTime.Now)).Hours} hrs {(DateTime.Now - (_context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id)?.CreatedAt ?? DateTime.Now)).Minutes} mins"
                        : $"{(DateTime.Now - (_context.OrderTableMappings.FirstOrDefault(otm => otm.TableId == table.Id)?.CreatedAt ?? DateTime.Now)).Minutes} mins")
                    : "N/A"
                }).ToList()
            };
            accordianItems.Add(accordianItem);
        }
        OrderAppViewModel orderAppViewModel = new OrderAppViewModel
        {
            Sections = accordianItems
        };
        return orderAppViewModel;
    }

    public IActionResult AddToWaitingList(WaitingListModal waitingListModal, int userId)
    {
        if (waitingListModal.Id == -1)
        {
            string name = waitingListModal.Name;
            string email = waitingListModal.Email;
            string mobileNumber = waitingListModal.MobileNumber;
            string numberOfPeople = waitingListModal.NumberOfPeople.ToString();
            string sectionId = waitingListModal.SectionId.ToString();
            if (_context.WaitingLists.Any(w => w.Customer.Email == email && w.IsDeleted == false))
            {
                return new JsonResult(new { success = false, message = "Customer already in waiting list" });
            }
            Customer? customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null && _context.Orders.Any(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has a ongoing order" });
            }
            if (customer == null)
            {
                customer = new Customer
                {
                    Name = name,
                    Email = email,
                    Phone = mobileNumber,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            WaitingList waitingList = new WaitingList
            {
                CustomerId = customer.Id,
                SectionId = int.Parse(sectionId),
                NoOfPersons = (short)int.Parse(numberOfPeople),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedBy = userId,
            };
            _context.WaitingLists.Add(waitingList);
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Added to waiting list successfully" });
        }
        else
        {
            WaitingList waitingList = _context.WaitingLists.Find(waitingListModal.Id);
            if (waitingList == null)
            {
                return new JsonResult(new { success = false, message = "Waiting list not found" });
            }
            Customer customer = _context.Customers.Find(waitingList.CustomerId);
            customer.Name = waitingListModal.Name;
            customer.Email = waitingListModal.Email;
            customer.Phone = waitingListModal.MobileNumber;
            customer.UpdatedBy = userId;
            waitingList.NoOfPersons = (short)waitingListModal.NumberOfPeople;
            waitingList.SectionId = waitingListModal.SectionId;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;
            _context.SaveChanges();
            return new JsonResult(new { success = true, message = "Waiting list updated successfully" });
        }
    }
    public JsonResult GetWaitingListForCurrentSection(int sectionId)
    {
        List<WaitingList> waitingLists = _context.WaitingLists.Where(w => w.SectionId == sectionId && w.IsDeleted == false).ToList();
        List<WaitingListTable> waitingListTables = new List<WaitingListTable>();
        foreach (WaitingList waitingList in waitingLists)
        {
            Customer customer = _context.Customers.Find(waitingList.CustomerId);
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
        return new JsonResult(new { success = true, customerDetails = waitingListTables });
    }

    public IActionResult AssignTablesToCustomer(WaitingListModal waitingListModal, List<int> tableIds, int userId)
    {
        if (tableIds.Count == 1)
        {
            Table table = _context.Tables.Find(tableIds[0]);
            if (table.Capacity < waitingListModal.NumberOfPeople) {
                return new JsonResult(new { success = false, message = "Customers can't be managed in selected table" });
            }
        }
        Customer customer = new Customer();
        foreach (int tableId in tableIds)
        {
            Table table = _context.Tables.Find(tableId);
            if (table.Capacity > waitingListModal.NumberOfPeople && tableIds.Count > 1)
                return new JsonResult(new { success = false, message = "Customers can be managed in less than selected tables" });
        }
        if (waitingListModal.Id == -1)
        {
            if (_context.WaitingLists.Any(w => w.Customer.Email == waitingListModal.Email && w.IsDeleted == false))
            {
                if (_context.WaitingLists.FirstOrDefault(w => w.Customer.Email == waitingListModal.Email && w.IsDeleted == false).SectionId == waitingListModal.SectionId)
                {
                    return new JsonResult(new { success = false, message = "Assign customer from waiting list" });
                } else {
                    return new JsonResult(new { success = false, message = "Customer already in waiting list of another section" });
                }
            }
            customer = _context.Customers.FirstOrDefault(c => c.Email == waitingListModal.Email);
            if (_context.Orders.Any(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has a ongoing order" });
            }
            if (customer == null)
            {
                customer = new Customer
                {
                    Name = waitingListModal.Name,
                    Email = waitingListModal.Email,
                    Phone = waitingListModal.MobileNumber,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
        }
        else
        {
            WaitingList waitingList = _context.WaitingLists.Find(waitingListModal.Id);
            customer = _context.Customers.Find(waitingList.CustomerId);
            customer.Name = waitingListModal.Name;
            customer.Email = waitingListModal.Email;
            customer.Phone = waitingListModal.MobileNumber;
            customer.UpdatedBy = userId;
            if (_context.Orders.Any(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has a ongoing order" });
            }
            waitingList.IsDeleted = true;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;
            waitingList.NoOfPersons = (short)waitingListModal.NumberOfPeople;
            _context.SaveChanges();
        }
        DAL.Models.Order order = new DAL.Models.Order
        {
            CustomerId = customer.Id,
            TotalAmount = 0,
            Status = "Pending",
            PaymentMode = "Cash",
            CreatedAt = DateTime.Now,
            CreatedBy = userId,
            UpdatedBy = userId,
            IsDeleted = false
        };
        _context.Orders.Add(order);
        _context.SaveChanges();
        foreach (int tableId in tableIds)
        {
            OrderTableMapping orderTableMapping = new OrderTableMapping
            {
                OrderId = order.Id,
                TableId = tableId,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId,
                IsDeleted = false,
                NoOfPersons = (short)waitingListModal.NumberOfPeople
            };
            Table table = _context.Tables.Find(tableId);
            table.Status = "Assigned";
            table.UpdatedBy = userId;
            _context.OrderTableMappings.Add(orderTableMapping);
            _context.SaveChanges();
        }
        return new JsonResult(new { success = true, message = "Tables assigned successfully" });
    }
}