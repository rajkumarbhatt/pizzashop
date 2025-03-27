using BLL.Interfaces;
using ClosedXML.Excel;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers
{

    [CustomAuth]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            CustomerViewModel customerViewModel = _customerService.GetCustomerDetails();
            return View(customerViewModel);
        }

        [HttpGet]
        public IActionResult FilterCustomers(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
        {
            CustomerViewModel customerViewModel = _customerService.FilterCustomers(pageIndex, pageSize, searchValue, time, sort, order, fromDate, toDate);
            return PartialView("_CustomersList", customerViewModel);
        }

        [HttpGet]
        public IActionResult ExportCustomers(string time, string searchValue, string fromDate, string toDate)
        {
            try
            {
                var customers = _customerService.GetCustomersBassedOnFilters(time, searchValue, fromDate, toDate);

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
                    worksheet.Range("E9:H9").Merge();
                    worksheet.Range("I9:K9").Merge();
                    worksheet.Range("L9:N9").Merge();
                    worksheet.Range("O9:P9").Merge();

                    int count = customers.Count, row2 = 10;

                    while (count > 0)
                    {
                        worksheet.Range($"B{row2}:D{row2}").Merge();
                        worksheet.Range($"E{row2}:H{row2}").Merge();
                        worksheet.Range($"I{row2}:K{row2}").Merge();
                        worksheet.Range($"L{row2}:N{row2}").Merge();
                        worksheet.Range($"O{row2}:P{row2}").Merge();
                        row2++;
                        count--;
                    }



                    worksheet.Cell("A2").Value = "Account:";
                    worksheet.Cell("C2").Value = "PizzaShop";
                    worksheet.Cell("H2").Value = "Search Text:";
                    worksheet.Cell("J2").Value = searchValue;

                    worksheet.Cell("A5").Value = "Date:";
                    worksheet.Cell("C5").Value = time;
                    worksheet.Cell("H5").Value = "No. Of Records:";
                    worksheet.Cell("J5").Value = customers.Count;

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
                    worksheet.Cell("B9").Value = "Name";
                    worksheet.Cell("E9").Value = "Email";
                    worksheet.Cell("I9").Value = "Date";
                    worksheet.Cell("L9").Value = "Mobile Number";
                    worksheet.Cell("O9").Value = "Total Order";

                    var headerRange = worksheet.Range("A9:O9");
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0066A7");
                    headerRange.Style.Font.FontColor = XLColor.White;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    int row = 10;
                    foreach (var customer in customers)
                    {
                        worksheet.Cell(row, 1).Value = customer.Id;
                        worksheet.Cell(row, 2).Value = customer.Name;
                        worksheet.Cell(row, 5).Value = customer.Email;
                        worksheet.Cell(row, 9).Value = customer.Date;
                        worksheet.Cell(row, 12).Value = customer.PhoneNumber;
                        worksheet.Cell(row, 15).Value = customer.TotalOrders;

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
        public IActionResult GetCustomerDetails(int id)
        {
            CustomerHistory customerHistory = _customerService.GetCustomerDetails(id);
            return PartialView("_CustomerDetails", customerHistory);
        }
    }
}