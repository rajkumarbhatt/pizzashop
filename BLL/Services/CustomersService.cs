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
            CustomerTable customerTable = new CustomerTable
            {
                Name = customer.Name,
                PhoneNumber = customer.Phone,
                Email = customer.Email,
                Date = _context.Orders.Where(o => o.CustomerId == customer.Id && o.IsDeleted == false).ToList().FirstOrDefault().CreatedAt.Value.ToString("dd/MM/yyyy") ?? "N/A"
            };
            customers.Add(customerTable);
        }
        return customers;
    }
}
