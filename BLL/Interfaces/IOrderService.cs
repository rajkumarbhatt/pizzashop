using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderService
{
    public Task<OrderViewModal> GetOrdersAsync();
    public Task<OrderViewModal> FilterOrdersAsync(int pageSize, int pageIndex, string status, string time, string sort, string order, string fromDate, string toDate, string searchValue);
    public Task<List<OrderTable>> GetOrdersBasedOnFiltersAsync(string status, string time, string searchValue);
    public Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId);
    public Task<byte[]> GenerateInvoiceAsync(int orderId);
    public Task<byte[]> ExportOrdersAsync(string status, string time, string searchValue, string fromDate, string toDate);
    public Task<string> EncryptOrderIdAsync(int orderId);
    public Task<int> DecryptOrderIdAsync(string orderIdEncrypted);
}
