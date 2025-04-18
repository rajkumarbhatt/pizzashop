using DAL.ViewModels;

namespace BLL.Interfaces;
public interface ICustomerService
{
    public Task<List<CustomerTable>> GetCustomersAsync();
    public Task<CustomerViewModel> GetCustomerDetailsTableAsync();
    public Task<CustomerViewModel> FilterCustomersAsync(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate);
    public Task<List<CustomerTable>> GetCustomersBasedOnFiltersAsync(string time, string searchValue, string fromDate, string toDate);
    public Task<CustomerHistory> GetCustomerDetailsAsync(int customerId);
    public Task<byte[]> ExportCustomersAsync(string time, string searchValue, string fromDate, string toDate);
}