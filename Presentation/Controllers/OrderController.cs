using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }
}
