using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderService
{
    public OrderViewModal GetOrders();
}
