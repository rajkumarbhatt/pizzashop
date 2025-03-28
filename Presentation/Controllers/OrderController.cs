using BLL.Interfaces;
using ClosedXML.Excel;
using DAL.DBContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers;

[CustomAuth]
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

    [HttpGet]
    public IActionResult FilterOrders(int pageSize, int pageIndex, string status, string time, string sort, string order, string fromDate, string toDate, string searchValue = null)
    {
        OrderViewModal orderViewModal = _orderService.FilterOrders(pageSize, pageIndex, status, time, sort, order, fromDate, toDate, searchValue);
        return PartialView("_OrderList", orderViewModal);
    }

    [HttpGet]
    public IActionResult ExportOrders(string status, string time, string searchValue)
    {
        return File(_orderService.ExportOrders(status, time, searchValue), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [HttpGet]
    [Route("Order/OrderDetails/{orderId}")]
    public IActionResult OrderDetails(int orderId)
    {
        OrderDetailsViewModel orderDetailsViewModel = _orderService.GetOrderDetails(orderId);
        return View(orderDetailsViewModel);
    }

    [HttpGet]
    public IActionResult DownloadInvoice(int orderId)
    {
        byte[] pdfBytes = _orderService.GenerateInvoice(orderId);
        return File(pdfBytes, "application/pdf", "Invoice.pdf");
    }
}