using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class OrderAppService : IOrderAppService
{
    private readonly PizzaShopContext _context;

    public OrderAppService(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<OrderAppViewModel> GetOrderAppViewModelAsync()
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

                string currentOrderTime = (DateTime.Now - createdAt).Days > 0
                    ? $"{(DateTime.Now - createdAt).Days} days {(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                    : (DateTime.Now - createdAt).Hours > 0
                    ? $"{(DateTime.Now - createdAt).Hours} hrs {(DateTime.Now - createdAt).Minutes} mins"
                    : $"{(DateTime.Now - createdAt).Minutes} mins";

                tableCards.Add(new TableCard
                {
                    OrderId = orderTableMapping?.OrderId ?? 0,
                    TableId = table.Id,
                    TableName = table.Name,
                    TableStatus = table.Status,
                    TableCapacity = table.Status == "Available"
                        ? table.Capacity
                        : orderTableMapping?.NoOfPersons ?? 0,
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

    public async Task<IActionResult> AddToWaitingListAsync(WaitingListModal waitingListModal, int userId)
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
                return new JsonResult(new { success = false, message = "Customer already in waiting list" });
            }

            Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer != null && await _context.Orders.AnyAsync(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has an ongoing order" });
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
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
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
            await _context.WaitingLists.AddAsync(waitingList);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Added to waiting list successfully" });
        }
        else
        {
            WaitingList waitingList = await _context.WaitingLists.FindAsync(waitingListModal.Id);
            if (waitingList == null)
            {
                return new JsonResult(new { success = false, message = "Waiting list not found" });
            }

            Customer customer = await _context.Customers.FindAsync(waitingList.CustomerId);
            customer.Name = waitingListModal.Name;
            customer.Email = waitingListModal.Email;
            customer.Phone = waitingListModal.MobileNumber;
            customer.UpdatedBy = userId;

            waitingList.NoOfPersons = (short)waitingListModal.NumberOfPeople;
            waitingList.SectionId = waitingListModal.SectionId;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true, message = "Waiting list updated successfully" });
        }
    }

    public async Task<JsonResult> GetWaitingListForCurrentSectionAsync(int sectionId)
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

        return new JsonResult(new { success = true, customerDetails = waitingListTables });
    }

    public async Task<IActionResult> AssignTablesToCustomerAsync(WaitingListModal waitingListModal, List<int> tableIds, int userId)
    {
        if (tableIds.Count == 1)
        {
            Table table = await _context.Tables.FindAsync(tableIds[0]);
            if (table.Capacity < waitingListModal.NumberOfPeople)
            {
                return new JsonResult(new { success = false, message = "Customers can't be managed in selected table" });
            }
        }

        Customer customer = new Customer();
        foreach (int tableId in tableIds)
        {
            Table table = await _context.Tables.FindAsync(tableId);
            if (table.Capacity > waitingListModal.NumberOfPeople && tableIds.Count > 1)
            {
                return new JsonResult(new { success = false, message = "Customers can be managed in less than selected tables" });
            }
        }

        if (tableIds.Count >= 1)
        {
            List<Table> tables = await _context.Tables.Where(t => t.IsDeleted == false && t.SectionId == waitingListModal.SectionId && t.Capacity >= waitingListModal.NumberOfPeople && t.Status == "Available" && tableIds.Contains(t.Id) == false).ToListAsync();
            if (tables == null || tables.Count == 0)
            {
                goto assign;
            }
            Table optimumTable = tables.OrderBy(t => t.Capacity).FirstOrDefault() ?? new Table();
            if (tableIds.Count == 1)
            {
                Table table = await _context.Tables.FindAsync(tableIds[0]) ?? new Table();
                if (table.Capacity == optimumTable.Capacity)
                {
                    goto assign;
                }
            }
            return new JsonResult(new { success = false, message = "You can assign " + optimumTable.Name + " for optimal arrangement" });
        }
    assign: 
        int capacity = 0;
        foreach (int tableId in tableIds)
        {
            Table table = await _context.Tables.FindAsync(tableId);
            capacity += table.Capacity;
        }

        if (capacity < waitingListModal.NumberOfPeople)
            return new JsonResult(new { success = false, message = "Customers can't be managed in selected tables" });

        if (waitingListModal.Id == -1)
        {
            if (await _context.WaitingLists.AnyAsync(w => w.Customer.Email == waitingListModal.Email && w.IsDeleted == false))
            {
                if ((await _context.WaitingLists.FirstOrDefaultAsync(w => w.Customer.Email == waitingListModal.Email && w.IsDeleted == false)).SectionId == waitingListModal.SectionId)
                {
                    return new JsonResult(new { success = false, message = "Assign customer from waiting list" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "Customer already in waiting list of another section" });
                }
            }

            customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == waitingListModal.Email);
            if (await _context.Orders.AnyAsync(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has an ongoing order" });
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
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            WaitingList waitingList = await _context.WaitingLists.FindAsync(waitingListModal.Id);
            customer = await _context.Customers.FindAsync(waitingList.CustomerId);
            customer.Name = waitingListModal.Name;
            customer.Email = waitingListModal.Email;
            customer.Phone = waitingListModal.MobileNumber;
            customer.UpdatedBy = userId;

            if (await _context.Orders.AnyAsync(o => o.CustomerId == customer.Id && (o.Status == "Pending" || o.Status == "In Progress" || o.Status == "Served")))
            {
                return new JsonResult(new { success = false, message = "Customer already has an ongoing order" });
            }

            waitingList.IsDeleted = true;
            waitingList.UpdatedAt = DateTime.Now;
            waitingList.UpdatedBy = userId;
            waitingList.NoOfPersons = (short)waitingListModal.NumberOfPeople;
            await _context.SaveChangesAsync();
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
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

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

            Table table = await _context.Tables.FindAsync(tableId);
            table.Status = "Assigned";
            table.UpdatedBy = userId;

            await _context.OrderTableMappings.AddAsync(orderTableMapping);
            await _context.SaveChangesAsync();
        }

        return new JsonResult(new { success = true, message = "Tables assigned successfully", orderId = order.Id });
    }
}