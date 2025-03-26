using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderService
{
    public OrderViewModal GetOrders();
    public OrderViewModal FilterOrders(int pageSize, int pageIndex, string status, string time, string sort, string order, string fromDate, string toDate, string searchValue);
    public List<OrderTable> GetOrdersBasedOnFilters(string status, string time, string searchValue);
    public OrderDetailsViewModel GetOrderDetails(int orderId);
    public List<InvoiceTax> GetInvoiceTaxes(int orderId, OrderDetailsViewModel orderDetailsViewModel);
    public byte[] GenerateInvoice(int orderId);
}
