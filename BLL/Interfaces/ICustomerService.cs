using System.Threading.Tasks;
using DAL.ViewModels;

namespace BLL.Interfaces;
public interface ICustomerService
{
    Task<List<CustomerTable>> GetCustomersAsync();
    Task<CustomerViewModel> GetCustomerDetailsAsync();
    Task<CustomerViewModel> FilterCustomersAsync(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate);
    Task<List<CustomerTable>> GetCustomersBassedOnFiltersAsync(string time, string searchValue, string fromDate, string toDate);
    Task<CustomerHistory> GetCustomerDetailsAsync(int customerId);
    Task<byte[]> ExportCustomersAsync(string time, string searchValue, string fromDate, string toDate);
}