using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Services;
public class CustomersService : ICustomerService
{
    private readonly PizzaShopContext _context;
    public CustomersService(PizzaShopContext context)
    {
        _context = context;
    }

    public List<CustomerTable> GetCustomers()
    {
        List<CustomerTable> customers = new List<CustomerTable>();
        List<Customer> customerList = _context.Customers.ToList();
        foreach (Customer customer in customerList)
        {
            string date = "01/01/1980";
            int totalOrders = 0;
            List<Order> orders = _context.Orders.Where(o => o.CustomerId == customer.Id && o.IsDeleted == false).OrderByDescending(o => o.CreatedAt).ToList();
            if (orders.Count != 0)
            {
                date = orders.FirstOrDefault().CreatedAt.Value.ToString("dd/MM/yyyy");
                totalOrders = orders.Count;
            }
            CustomerTable customerTable = new CustomerTable
            {
                Id = customer.Id,
                Name = customer.Name ?? "N/A",
                PhoneNumber = customer.Phone ?? "N/A",
                Email = customer.Email ?? "N/A",
                Date = date ?? "N/A",
                TotalOrders = totalOrders
            };
            customers.Add(customerTable);
        }
        customers = customers.OrderBy(c => c.Name).ToList();
        return customers;
    }

    public CustomerViewModel GetCustomerDetails()
    {

        List<CustomerTable> customers = GetCustomers();
        CustomerViewModel customerViewModel = new CustomerViewModel
        {
            Customers = customers.Skip(0).Take(5).ToList(),
            pageIndex = 1,
            pageSize = 5,
            totalCustomers = customers.Count,
            totalPages = (int)Math.Ceiling((decimal)customers.Count / 5)
        };
        return customerViewModel;
    }

    public CustomerViewModel FilterCustomers(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;

        List<CustomerTable> customers = GetCustomers();

        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                customers = customers.Where(c => c.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
            else if (time == "Custom Date")
            {
                if (fromDate != null && toDate != null)
                {
                    customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Parse(fromDate) && DateTime.Parse(c.Date) <= DateTime.Parse(toDate)).ToList();
                }
            }
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            customers = customers.Where(c => c.Name.ToLower().Contains(searchValue)).ToList();
        }
        if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
        {
            if (sort == "name")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.Name).ToList() : customers.OrderByDescending(c => c.Name).ToList();
            }
            else if (sort == "date")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.Date).ToList() : customers.OrderByDescending(c => c.Date).ToList();
            }
            else if (sort == "totalOrders")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.TotalOrders).ToList() : customers.OrderByDescending(c => c.TotalOrders).ToList();
            }
        }

        int totalCustomers = customers.Count;
        int totalPages = (int)Math.Ceiling((decimal)totalCustomers / pageSize);
        CustomerViewModel customerViewModel = new CustomerViewModel
        {
            Customers = customers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            pageIndex = pageIndex,
            pageSize = pageSize,
            totalCustomers = totalCustomers,
            totalPages = totalPages
        };
        return customerViewModel;
    }

    public List<CustomerTable>  GetCustomersBassedOnFilters(string time, string searchValue, string fromDate, string toDate)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;

        List<CustomerTable> customers = GetCustomers();

        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                customers = customers.Where(c => c.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
            else if (time == "Custom Date")
            {
                if (fromDate != null && toDate != null)
                {
                    customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Parse(fromDate) && DateTime.Parse(c.Date) <= DateTime.Parse(toDate)).ToList();
                }
            }
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            customers = customers.Where(c => c.Name.ToLower().Contains(searchValue)).ToList();
        }
        
        return customers;
    }

    public CustomerHistory GetCustomerDetails(int customerId)
    {
        Customer customer = _context.Customers.FirstOrDefault(c => c.Id == customerId) ?? new Customer();
        List<Order> orders = _context.Orders.Where(o => o.CustomerId == customerId && o.IsDeleted == false).OrderByDescending(o => o.CreatedAt).ToList();
        List<OrderItem> orderItems = _context.OrderItems.Where(oi => orders.Select(o => o.Id).Contains(oi.OrderId)).ToList();
        double avaerageBill = (double)(orders.Count != 0 ? orders.Average(o => o.TotalAmount) : 0);
        string comingSince = orders.Count != 0 ? orders.LastOrDefault().CreatedAt.Value.ToString("dd/MM/yyyy hh:mm tt") : "N/A";
        string maxOrderAmount = orders.Count != 0 ? orders.Max(o => o.TotalAmount).ToString() : "N/A";
        CustomerHistory customerHistory = new CustomerHistory
        {
            Name = customer.Name,
            PhoneNumber = customer.Phone,
            AverageBill = avaerageBill,
            ComingSince = comingSince,
            MaxOrderAmount = maxOrderAmount,
            Visits = orders.Count,
            Orders = orders.Select(o => new CustomerHistoryOrderDetails
            {
                OrderDate = o.CreatedAt.Value.ToString("dd/MM/yyyy"),
                OrderType = "Dine In",
                OrderAmount = (double?)o.TotalAmount,
                PaymentType = o.PaymentMode,
                NumberOfItems = orderItems.Where(oi => oi.OrderId == o.Id).Count()
            }).ToList()
        };
        return customerHistory;
    }
}
