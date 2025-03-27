using DAL.ViewModels;

namespace BLL.Interfaces;
public interface ICustomerService
{
    public List<CustomerTable> GetCustomers();
    public CustomerViewModel GetCustomerDetails();
    public CustomerViewModel FilterCustomers (int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate);
    public List<CustomerTable> GetCustomersBassedOnFilters (string time, string searchValue, string fromDate, string toDate);
    public CustomerHistory GetCustomerDetails (int customerId);
}