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
        try
        {
            var orders = _orderService.GetOrdersBasedOnFilters(status, time, searchValue);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");

                worksheet.Range("A2:B3").Merge();
                worksheet.Range("C2:F3").Merge();
                worksheet.Range("H2:I3").Merge();
                worksheet.Range("J2:M3").Merge();
                worksheet.Range("A5:B6").Merge();
                worksheet.Range("C5:F6").Merge();
                worksheet.Range("H5:I6").Merge();
                worksheet.Range("J5:M6").Merge();
                worksheet.Range("B9:D9").Merge();
                worksheet.Range("E9:G9").Merge();
                worksheet.Range("H9:J9").Merge();
                worksheet.Range("K9:L9").Merge();
                worksheet.Range("M9:N9").Merge();
                worksheet.Range("O9:P9").Merge();

               int count = orders.Count, row2 = 10;

               while (count > 0) {
                    worksheet.Range($"B{row2}:D{row2}").Merge();
                    worksheet.Range($"E{row2}:G{row2}").Merge();
                    worksheet.Range($"H{row2}:J{row2}").Merge();
                    worksheet.Range($"K{row2}:L{row2}").Merge();
                    worksheet.Range($"M{row2}:N{row2}").Merge();
                    worksheet.Range($"O{row2}:P{row2}").Merge();
                    row2++;
                    count--;
               }

                

                worksheet.Cell("A2").Value = "Status:";
                worksheet.Cell("C2").Value = status;
                worksheet.Cell("H2").Value = "Search Text:";
                worksheet.Cell("J2").Value = searchValue;

                worksheet.Cell("A5").Value = "Date:";
                worksheet.Cell("C5").Value = time;
                worksheet.Cell("H5").Value = "No. Of Records:";
                worksheet.Cell("J5").Value = orders.Count;

                worksheet.Range("A2:B3").Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                worksheet.Range("A2:B3").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A2:B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A2:B3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A2:B3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("C2:F3").Style.Fill.BackgroundColor = XLColor.White;
                worksheet.Range("C2:F3").Style.Font.FontColor = XLColor.Black;
                worksheet.Range("C2:F3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("C2:F3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("C2:F3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("H2:I3").Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                worksheet.Range("H2:I3").Style.Font.FontColor = XLColor.White;
                worksheet.Range("H2:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("H2:I3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("H2:I3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("J2:M3").Style.Fill.BackgroundColor = XLColor.White;
                worksheet.Range("J2:M3").Style.Font.FontColor = XLColor.Black;
                worksheet.Range("J2:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("J2:M3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("J2:M3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("A5:B6").Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                worksheet.Range("A5:B6").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A5:B6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A5:B6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("A5:B6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("C5:F6").Style.Fill.BackgroundColor = XLColor.White;
                worksheet.Range("C5:F6").Style.Font.FontColor = XLColor.Black;
                worksheet.Range("C5:F6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("C5:F6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("C5:F6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                

                worksheet.Range("H5:I6").Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                worksheet.Range("H5:I6").Style.Font.FontColor = XLColor.White;
                worksheet.Range("H5:I6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("H5:I6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("H5:I6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range("J5:M6").Style.Fill.BackgroundColor = XLColor.White;
                worksheet.Range("J5:M6").Style.Font.FontColor = XLColor.Black;
                worksheet.Range("J5:M6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("J5:M6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Range("J5:M6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell("A9").Value = "Id";
                worksheet.Cell("B9").Value = "Date";
                worksheet.Cell("E9").Value = "Customer";
                worksheet.Cell("H9").Value = "Status";
                worksheet.Cell("K9").Value = "Payment Mode";
                worksheet.Cell("M9").Value = "Rating";
                worksheet.Cell("O9").Value = "Total Amount";

                var headerRange = worksheet.Range("A9:O9");
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                int row = 10;
                foreach (var order in orders)
                {
                    worksheet.Cell(row, 1).Value = order.Id;
                    worksheet.Cell(row, 2).Value = order.Date;
                    worksheet.Cell(row, 5).Value = order.CustomerName;
                    worksheet.Cell(row, 8).Value = order.Status;
                    worksheet.Cell(row, 11).Value = order.PaymentMode;
                    worksheet.Cell(row, 13).Value = order.AvgRating;
                    worksheet.Cell(row, 15).Value = order.TotalAmount;

                    var dataRowRange = worksheet.Range(row, 1, row, 15);
                    dataRowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    dataRowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    dataRowRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    
                    worksheet.Range($"O9:P{row - 1}").Style.Border.RightBorder = XLBorderStyleValues.Thin;


                    row++;
                }

                worksheet.Cell(row - 1, 16).Style.Border.RightBorder = XLBorderStyleValues.Thin;

                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");
                if (System.IO.File.Exists(imagePath))
                {
                    var picture = worksheet.AddPicture(imagePath).MoveTo(worksheet.Cell("O2")).Scale(0.3);
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    [Route("Order/OrderDetails/{orderId}")]
    public IActionResult OrderDetails (int orderId) {
        OrderDetailsViewModel orderDetailsViewModel = _orderService.GetOrderDetails(orderId);
        return View(orderDetailsViewModel);
    }

    [HttpGet]
    public IActionResult DownloadInvoice (int orderId) {
        byte[] pdfBytes =_orderService.GenerateInvoice(orderId);
        return File(pdfBytes, "application/pdf", "Invoice.pdf");
    }
}