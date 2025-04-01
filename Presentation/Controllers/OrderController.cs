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
    private readonly IJwtService _jwtService;

    public OrderController(IOrderService orderService, PizzaShopContext context, IJwtService jwtService)
    {
        _orderService = orderService;
        _context = context;
        _jwtService = jwtService;
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
    [Route("Order/OrderDetails/{orderIdEncrypted}")]
    public IActionResult OrderDetails(string orderIdEncrypted)
    {
        int orderId = _orderService.DecryptOrderId(orderIdEncrypted);
        OrderDetailsViewModel orderDetailsViewModel = _orderService.GetOrderDetails(orderId);
        return View(orderDetailsViewModel);
    }

    [HttpGet]
    public IActionResult DownloadInvoice(int orderId)
    {
        byte[] pdfBytes = _orderService.GenerateInvoice(orderId);
        return File(pdfBytes, "application/pdf", "Invoice.pdf");
    }

    [HttpGet]
    public IActionResult EncryptOrder(int orderId)
    {
        string encryptedOrderId = _orderService.EncryptOrderId(orderId);
        return Json(new { encryptedOrderId });
    }
}