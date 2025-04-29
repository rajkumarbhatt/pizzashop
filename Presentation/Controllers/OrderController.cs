using BLL.Interfaces;
using ClosedXML.Excel;
using DAL.DBContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers;

[CustomAuth]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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

    public async Task<IActionResult> Index()
    {
        OrderViewModal orderViewModal = await _orderService.GetOrdersAsync();
        return View(orderViewModal);
    }

    [HttpGet]
    public async Task<IActionResult> FilterOrders(int pageSize, int pageIndex, string status, string time, string sort, string order, string fromDate, string toDate, string searchValue = null)
    {
        OrderViewModal orderViewModal = await _orderService.FilterOrdersAsync(pageSize, pageIndex, status, time, sort, order, fromDate, toDate, searchValue);
        return PartialView("_OrderList", orderViewModal);
    }

    [HttpGet]
    public async Task<IActionResult> ExportOrders(string status, string time, string searchValue, string fromDate, string toDate)
    {
        byte[] fileBytes = await _orderService.ExportOrdersAsync(status, time, searchValue, fromDate, toDate);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    [HttpGet]
    [Route("Order/OrderDetails/{orderIdEncrypted}")]
    public async Task<IActionResult> OrderDetails(string orderIdEncrypted)
    {
        int orderId = await _orderService.DecryptOrderIdAsync(orderIdEncrypted);
        OrderDetailsViewModel orderDetailsViewModel = await _orderService.GetOrderDetailsAsync(orderId);
        return View(orderDetailsViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadInvoice(int orderId)
    {
        byte[] pdfBytes = await _orderService.GenerateInvoiceAsync(orderId);
        return File(pdfBytes, "application/pdf", "Invoice.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> EncryptOrder(int orderId)
    {
        string encryptedOrderId = await _orderService.EncryptOrderIdAsync(orderId);
        return Json(new { encryptedOrderId });
    }
}