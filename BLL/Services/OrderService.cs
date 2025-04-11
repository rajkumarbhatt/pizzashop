using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using iText.Kernel.Pdf;
using iText.Html2pdf;
using ClosedXML.Excel;
using System.Text;
using iText.Layout.Font;
using Microsoft.EntityFrameworkCore;


namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly PizzaShopContext _context;
    public OrderService(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<OrderViewModal> GetOrdersAsync()
    {
        List<Order> orders = await _context.Orders.ToListAsync();

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order in orders)
        {
            Customer customer = await _context.Customers.FindAsync(order.CustomerId) ?? new Customer();
            CustomerReview customerReview = await _context.CustomerReviews
                .Where(cr => cr.CustomerId == order.CustomerId && cr.OrderId == order.Id)
                .FirstOrDefaultAsync() ?? new CustomerReview();

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
        orderTables = orderTables.OrderBy(o => o.Id).ToList();
        int pageIndex = 1;
        int pageSize = 5;
        int totalOrders = orderTables.Count;
        int totalPages = (int)Math.Ceiling((decimal)totalOrders / pageSize);
        OrderViewModal orderViewModal = new OrderViewModal
        {
            Orders = orderTables.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            pageIndex = pageIndex,
            pageSize = pageSize,
            totalOrders = totalOrders,
            totalPages = totalPages
        };
        return orderViewModal;
    }
    public async Task<OrderViewModal> FilterOrdersAsync(int pageSize, int pageIndex, string status, string time, string sort, string order, string? fromDate, string? toDate, string searchValue)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;
        List<Order> orders = await _context.Orders.ToListAsync();

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order2 in orders)
        {
            Customer customer = await _context.Customers.FindAsync(order2.CustomerId) ?? new Customer();
            CustomerReview customerReview = await _context.CustomerReviews
                .Where(cr => cr.CustomerId == order2.CustomerId && cr.OrderId == order2.Id)
                .FirstOrDefaultAsync() ?? new CustomerReview();

            OrderTable orderTable = new OrderTable
            {
                Id = order2.Id,
                Date = order2.CreatedAt.HasValue ? order2.CreatedAt.Value.ToString("dd/MM/yyyy") : "N/A",
                CustomerName = customer.Name ?? "N/A",
                Status = order2.Status ?? "N/A",
                PaymentMode = order2.PaymentMode ?? "N/A",
                AvgRating = (double)(customerReview.AverageRating ?? 0),
                TotalAmount = order2.TotalAmount
            };

            orderTables.Add(orderTable);
        }
        if (!string.IsNullOrEmpty(status) && status != "All Status")
        {
            orderTables = orderTables.Where(o => o.Status == status).ToList();
        }
        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                orderTables = orderTables.Where(o => o.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                orderTables = orderTables.Where(o => DateTime.Parse(o.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                orderTables = orderTables.Where(o => DateTime.Parse(o.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
        }
        if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
        {
            orderTables = orderTables.Where(o => DateTime.Parse(o.Date) >= DateTime.Parse(fromDate) && DateTime.Parse(o.Date) <= DateTime.Parse(toDate)).ToList();
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            orderTables = orderTables.Where(o => o.CustomerName.ToLower().Contains(searchValue.ToLower())).ToList();
        }
        if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
        {
            if (sort == "id")
            {
                orderTables = order == "asc" ? orderTables.OrderBy(o => o.Id).ToList() : orderTables.OrderByDescending(o => o.Id).ToList();
            }
            else if (sort == "date")
            {
                orderTables = order == "asc" ? orderTables.OrderBy(o => DateTime.Parse(o.Date)).ToList() : orderTables.OrderByDescending(o => DateTime.Parse(o.Date)).ToList();
            }
            else if (sort == "customerName")
            {
                orderTables = order == "asc" ? orderTables.OrderBy(o => o.CustomerName).ToList() : orderTables.OrderByDescending(o => o.CustomerName).ToList();
            }
            else if (sort == "totalAmount")
            {
                orderTables = order == "asc" ? orderTables.OrderBy(o => o.TotalAmount).ToList() : orderTables.OrderByDescending(o => o.TotalAmount).ToList();
            }
        }
        int totalOrders = orderTables.Count;
        int totalPages = (int)Math.Ceiling((decimal)totalOrders / pageSize);
        OrderViewModal orderViewModal = new OrderViewModal
        {
            Orders = orderTables.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            pageIndex = pageIndex,
            pageSize = pageSize,
            totalOrders = totalOrders,
            totalPages = totalPages
        };
        return orderViewModal;
    }
    public async Task<List<OrderTable>> GetOrdersBasedOnFiltersAsync(string status, string time, string searchValue)
    {
        List<Order> orders = await _context.Orders.ToListAsync();
        List<Customer> customers = await _context.Customers.ToListAsync();
        List<CustomerReview> customerReviews = await _context.CustomerReviews.ToListAsync();

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order in orders)
        {
            Customer customer = customers.Find(c => c.Id == order.CustomerId) ?? new Customer();
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
        if (!string.IsNullOrEmpty(status) && status != "All Status")
        {
            orderTables = orderTables.Where(o => o.Status == status).ToList();
        }
        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                orderTables = orderTables.Where(o => o.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                orderTables = orderTables.Where(o => DateTime.Parse(o.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                orderTables = orderTables.Where(o => DateTime.Parse(o.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            orderTables = orderTables.Where(o => o.CustomerName.ToLower().Contains(searchValue.ToLower())).ToList();
        }
        return orderTables;
    }
    public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(int orderId)
    {
        int srNo = 1;
        OrderDetailsViewModel orderDetailsViewModel = await _context.Orders.Where(o => o.Id == orderId).Select(o => new OrderDetailsViewModel
        {
            Id = o.Id,
            InvoiceNumber = "N/A",
            PaidOn = "N/A",
            ModifiedOn = o.UpdatedAt.HasValue ? o.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A",
            PlacedOn = o.CreatedAt.HasValue ? o.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A",
            OrderDuration = "N/A",
            OrderStatus = o.Status ?? "N/A",
            CustomerName = o.Customer.Name ?? "N/A",
            CustomerEmail = o.Customer.Email ?? "N/A",
            CustomerPhone = o.Customer.Phone ?? "N/A",
            NumberOfPeople = 0,
            Tables = o.OrderTableMappings.Select(otm => otm.Table).ToList(),
            Section = o.OrderTableMappings.Select(otm => otm.Table.Section.Name).FirstOrDefault() ?? "N/A",
            InvoiceItems = o.OrderItems.Where(oi => oi.IsDeleted == false).Select(oi => new InvoiceItem
            {
                SrNo = srNo,
                Item = oi.Item.Name ?? "N/A",
                Quantity = oi.Quantity,
                Price = (double)(oi.Price ?? 0),
                TotalAmount = (double)(oi.Price ?? 0) * oi.Quantity,
                InvoiceModifiers = oi.OrderModifiers.Where(om => om.IsDeleted == false).Select(om => new InvoiceModifiers
                {
                    Item = om.Modifier.Name ?? "N/A",
                    Quantity = (int)om.Quantity,
                    Price = (double)(om.Price ?? 0),
                    TotalAmount = (double)(om.Price * om.Quantity ?? 0)
                }).ToList()
            }).ToList(),
            SubTotal = 0,
            InvoiceTaxes = o.OrderTaxes.Select(ot => new InvoiceTax
            {
                TaxName = ot.Tax.Name ?? "N/A",
                TaxAmount = (double)ot.TaxAmount
            }).ToList(),
            Total = 0,
        }).FirstOrDefaultAsync() ?? new OrderDetailsViewModel();

        orderDetailsViewModel.InvoiceNumber = "INV" + orderDetailsViewModel.Id;

        foreach (InvoiceItem i in orderDetailsViewModel.InvoiceItems ?? new List<InvoiceItem>())
        {
            orderDetailsViewModel.SubTotal += i.TotalAmount;
            foreach (InvoiceModifiers im in i.InvoiceModifiers ?? new List<InvoiceModifiers>())
            {
                orderDetailsViewModel.SubTotal += im.TotalAmount;
            }
        }

        List<OrderTableMapping> otmap = await _context.OrderTableMappings.Where(otm => otm.OrderId == orderId).ToListAsync();
        int NumberOfPeople = 0;
        foreach (OrderTableMapping otm in otmap)
        {
            NumberOfPeople += otm.NoOfPersons;
        }
        orderDetailsViewModel.NumberOfPeople = NumberOfPeople;

        orderDetailsViewModel.OrderDuration = (DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).ToString();
        orderDetailsViewModel.OrderDuration = orderDetailsViewModel.OrderDuration.Substring(0, orderDetailsViewModel.OrderDuration.LastIndexOf("."));
        var days = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalDays;
        var hours = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalHours;
        var minutes = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalMinutes;

        if (days > 0)
        {
            orderDetailsViewModel.OrderDuration = days + " days " + hours % 24 + " hours " + minutes % 60 + " minutes ago";
        }
        else if (hours > 0)
        {
            orderDetailsViewModel.OrderDuration = hours + " hours " + minutes % 60 + " minutes ago";
        }
        else if (minutes > 0)
        {
            orderDetailsViewModel.OrderDuration = minutes % 60 + " minutes ago";
        }

        orderDetailsViewModel.SubTotal = (double?)Math.Round((decimal)(orderDetailsViewModel.SubTotal ?? 0), 2);
        orderDetailsViewModel.Total += orderDetailsViewModel.SubTotal;

        orderDetailsViewModel.InvoiceTaxes = await GetInvoiceTaxesAsync(orderId, orderDetailsViewModel);
        foreach (InvoiceTax i in orderDetailsViewModel.InvoiceTaxes)
        {
            orderDetailsViewModel.Total += i.TaxAmount;
        }
        orderDetailsViewModel.Total = (double?)Math.Round((decimal)(orderDetailsViewModel.Total ?? 0), 2);

        return orderDetailsViewModel;
    }
    public async Task<List<InvoiceTax>> GetInvoiceTaxesAsync(int orderId, OrderDetailsViewModel orderDetailsViewModel)
    {
        List<InvoiceTax> invoiceTaxes = new List<InvoiceTax>();
        Order order = await _context.Orders.FindAsync(orderId) ?? new Order();
        List<OrderTaxis> orderTaxes = await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToListAsync();
        foreach (OrderTaxis orderTax in orderTaxes)
        {
            TaxesFee tax = await _context.TaxesFees.FindAsync(orderTax.TaxId) ?? new TaxesFee();
            InvoiceTax invoiceTax = new InvoiceTax();
            if (tax.TaxType == "Percentage")
            {
                invoiceTax.TaxName = tax.Name;
                invoiceTax.TaxAmount = (double)orderTax.TaxAmount;
                invoiceTax.TaxAmount = Math.Round(invoiceTax.TaxAmount, 2);
            }
            else
            {
                invoiceTax.TaxName = tax.Name;
                invoiceTax.TaxAmount = (double)orderTax.TaxAmount;
            }
            invoiceTaxes.Add(invoiceTax);
        }
        return invoiceTaxes;
    }
    public async Task<byte[]> GenerateInvoiceAsync(int orderId)
    {
        string fontPath = "C:/Windows/Fonts/Arial.ttf";
        FontProvider fontProvider = new FontProvider();
        fontProvider.AddFont(fontPath);

        OrderDetailsViewModel orderDetailsViewModel = await GetOrderDetailsAsync(orderId);

        using (MemoryStream ms = new MemoryStream())
        {
            string htmlContent = await System.IO.File.ReadAllTextAsync("Views/Order/Invoicetemplate.html");
            htmlContent = htmlContent.Replace("{{invoiceNumber}}", orderDetailsViewModel.InvoiceNumber);
            htmlContent = htmlContent.Replace("{{invoiceDate}}", orderDetailsViewModel.PlacedOn);
            htmlContent = htmlContent.Replace("{{section}}", orderDetailsViewModel.Section);
            htmlContent = htmlContent.Replace("{{customerName}}", orderDetailsViewModel.CustomerName);
            htmlContent = htmlContent.Replace("{{customerPhone}}", orderDetailsViewModel.CustomerPhone);
            string tables = "";
            foreach (DAL.Models.Table table in orderDetailsViewModel.Tables ?? new List<DAL.Models.Table>())
            {
                tables += table.Name + ",";
            }
            tables = tables.TrimEnd(',');
            htmlContent = htmlContent.Replace("{{table}}", tables);
            string invoiceItems = "";
            int srNo = 1;
            foreach (InvoiceItem invoiceItem in orderDetailsViewModel.InvoiceItems ?? new List<InvoiceItem>())
            {
                string invoiceModifiers = "";
                foreach (InvoiceModifiers invoiceModifier in invoiceItem.InvoiceModifiers ?? new List<InvoiceModifiers>())
                {
                    invoiceModifiers += "<tr><td></td><td>" + invoiceModifier.Item + "</td><td>" + invoiceModifier.Quantity + "</td><td >" + invoiceModifier.Price + "</td><td style='text-align: right;'>" + invoiceModifier.TotalAmount + "</td></tr>";
                }
                invoiceItems += "<tr><td>" + srNo++ + "</td><td>" + invoiceItem.Item + "</td><td>" + invoiceItem.Quantity + "</td><td> " + invoiceItem.Price + "</td><td style='text-align: right;'>" + invoiceItem.TotalAmount + "</td></tr>" + invoiceModifiers;
            }
            htmlContent = htmlContent.Replace("{{invoiceItems}}", invoiceItems);
            string invoiceTaxes = "<tr><td></td><td>SubTotal</td><td></td><td></td><td style='text-align: right;'>" + orderDetailsViewModel.SubTotal + "</td></tr>";
            foreach (InvoiceTax invoiceTax in orderDetailsViewModel.InvoiceTaxes ?? new List<InvoiceTax>())
            {
                invoiceTaxes += "<tr><td></td><td>" + invoiceTax.TaxName + "</td><td></td><td></td><td style='text-align: right;'>" + invoiceTax.TaxAmount + "</td></tr>";
            }
            invoiceTaxes += "<tr><td></td><td>Total Amount Due</td><td></td><td></td><td style='text-align: right;'>" + orderDetailsViewModel.Total + "</td></tr>";
            htmlContent = htmlContent.Replace("{{invoiceTaxes}}", invoiceTaxes);
            htmlContent = htmlContent.Replace("{{paymentType}}", "N/A");
            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);
            HtmlConverter.ConvertToPdf(htmlContent, pdf, converterProperties);
            return ms.ToArray();
        }
    }
    public async Task<byte[]> ExportOrdersAsync(string status, string time, string searchValue)
    {
        var orders = await GetOrdersBasedOnFiltersAsync(status, time, searchValue);

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

            while (count > 0)
            {
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
                return content;
            }
        }
    }
    public async Task<string> EncryptOrderIdAsync(int orderId)
    {
        return await Task.Run(() =>
        {
            string encryptedOrderId = orderId.ToString();
            byte[] data = Encoding.UTF8.GetBytes(encryptedOrderId);
            encryptedOrderId = Convert.ToBase64String(data);
            return encryptedOrderId;
        });
    }
    public async Task<int> DecryptOrderIdAsync(string orderIdEncrypted)
    {
        return await Task.Run(() =>
        {
            string decryptedOrderId = orderIdEncrypted;
            byte[] data = Convert.FromBase64String(decryptedOrderId);
            decryptedOrderId = Encoding.UTF8.GetString(data);
            return Convert.ToInt32(decryptedOrderId);
        });
    }
}