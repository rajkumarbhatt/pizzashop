using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly PizzaShopContext _context;

    public OrderService(PizzaShopContext context)
    {
        _context = context;
    }

    public OrderViewModal GetOrders()
    {
        List<Order> orders = _context.Orders.ToList();
        List<Customer> customers = _context.Customers.ToList();
        List<CustomerReview> customerReviews = _context.CustomerReviews.ToList();

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order in orders)
        {
            Customer customer = customers.Find(c => c.Id == order.CustomerId);
            CustomerReview customerReview = customerReviews.Find(cr => cr.CustomerId == order.CustomerId && cr.OrderId == order.Id) ?? new CustomerReview();

            OrderTable orderTable = new OrderTable
            {
                Id = order.Id,
                Date = order.CreatedAt.HasValue ? order.CreatedAt.Value.ToString("dd/MM/yyyy") : "N/A",
                CustomerName = customer.Name ?? "N/A",
                Status = order.Status ?? "N/A",
                PaymentMode = order.PaymentMode ?? "N/A",
                AvgRating = (double)(customerReview.AverageRating ?? 0),
                TotalAmount = order.TotalAmount 
                
            };

            orderTables.Add(orderTable);
        }
        OrderViewModal orderViewModal = new OrderViewModal
        {
            Orders = orderTables
        };
        return orderViewModal;
    }
}
