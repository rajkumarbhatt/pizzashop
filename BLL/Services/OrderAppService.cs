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
                    TableCapacity = table.Capacity.ToString(),
                    CurentOrderTime = "N/A"
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

    public IActionResult AddToWaitingList(string email, string name, string mobileNumber, string sectionId, string numberOfPeople, int userId)
    {
        Customer customer = _context.Customers.FirstOrDefault(c => c.Email == email);
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
}