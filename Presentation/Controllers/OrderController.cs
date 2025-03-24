using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly PizzaShopContext _context;

    public OrderController(IOrderService orderService, PizzaShopContext context)
    {
        _orderService = orderService;
        _context = context;
    }

    public IActionResult Index()
    {
        OrderViewModal orderViewModal = _orderService.GetOrders();
        return View(orderViewModal);
    }
}
