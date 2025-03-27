using DAL.ViewModels;

namespace BLL.Interfaces;
public interface ICustomerService
{
    public List<CustomerTable> GetCustomers();
}