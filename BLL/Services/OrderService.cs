using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using iText.Kernel.Pdf;
using iText.Html2pdf;

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

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order in orders)
        {
            Customer customer = _context.Customers.Find(order.CustomerId) ?? new Customer();
            CustomerReview customerReview = _context.CustomerReviews.Where(cr => cr.CustomerId == order.CustomerId && cr.OrderId == order.Id).FirstOrDefault() ?? new CustomerReview();

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
    public OrderViewModal FilterOrders(int pageSize, int pageIndex, string status, string time, string sort, string order, string fromDate, string toDate, string searchValue)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;
        List<Order> orders = _context.Orders.ToList();

        List<OrderTable> orderTables = new List<OrderTable>();
        foreach (Order order2 in orders)
        {
            Customer customer = _context.Customers.Find(order2.CustomerId) ?? new Customer();
            CustomerReview customerReview = _context.CustomerReviews.Where(cr => cr.CustomerId == order2.CustomerId && cr.OrderId == order2.Id).FirstOrDefault() ?? new CustomerReview();

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
    public List<OrderTable> GetOrdersBasedOnFilters(string status, string time, string searchValue)
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
    public OrderDetailsViewModel GetOrderDetails(int orderId)
    {
        int srNo = 1;
        OrderDetailsViewModel orderDetailsViewModel = _context.Orders.Where(o => o.Id == orderId).Select(o => new OrderDetailsViewModel
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
            NumberOfPeople = o.OrderTableMappings.Select(otm => otm.Noofpersons).FirstOrDefault().ToString() ?? "N/A",
            Tables = o.OrderTableMappings.Select(otm => otm.Table).ToList(),
            Section = o.OrderTableMappings.Select(otm => otm.Table.Section.Name).FirstOrDefault() ?? "N/A",
            InvoiceItems = o.OrderItems.Select(oi => new InvoiceItem
            {
                SrNo = srNo,
                Item = oi.Item.Name ?? "N/A",
                Quantity = oi.Quantity,
                Price = (double)oi.Price,
                TotalAmount = (double)oi.Price * oi.Quantity,
                InvoiceModifiers = oi.OrderModifiers.Select(om => new InvoiceModifiers
                {
                    Item = om.Modifier.Name ?? "N/A",
                    Quantity = 1,
                    Price = (double)om.Price,
                    TotalAmount = (double)om.Price
                }).ToList()
            }).ToList(),
            SubTotal = 0,
            InvoiceTaxes = o.OrderTaxes.Select(ot => new InvoiceTax
            {
                TaxName = ot.Tax.Name ?? "N/A",
                TaxAmount = (double)ot.TaxAmount
            }).ToList(),
            Total = 0,
        }).FirstOrDefault() ?? new OrderDetailsViewModel();

        foreach (InvoiceItem i in orderDetailsViewModel.InvoiceItems)
        {
            orderDetailsViewModel.SubTotal += i.TotalAmount;
            foreach (InvoiceModifiers im in i.InvoiceModifiers)
            {
                orderDetailsViewModel.SubTotal += im.TotalAmount;
            }
        }
        orderDetailsViewModel.OrderDuration = (DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).ToString();
        orderDetailsViewModel.OrderDuration = orderDetailsViewModel.OrderDuration.Substring(0, orderDetailsViewModel.OrderDuration.LastIndexOf("."));
        var days = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalDays;
        var hours = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalHours;
        var minutes = (int)(DateTime.Now - Convert.ToDateTime(orderDetailsViewModel.PlacedOn)).TotalMinutes;
        if (days > 0)
        {
            orderDetailsViewModel.OrderDuration = days + " days " + hours%24 + " hours " + minutes%60 + " minutes ago";
        }
        else if (hours > 0)
        {
            orderDetailsViewModel.OrderDuration = hours + " hours " + minutes + " minutes ago";
        }
        else if (minutes > 0)
        {
            orderDetailsViewModel.OrderDuration = minutes + " minutes ago";
        }
        orderDetailsViewModel.SubTotal = (double?)Math.Round((decimal)orderDetailsViewModel.SubTotal, 2);
        orderDetailsViewModel.Total += orderDetailsViewModel.SubTotal;
        orderDetailsViewModel.InvoiceTaxes = GetInvoiceTaxes(orderId, orderDetailsViewModel);
        foreach (InvoiceTax i in orderDetailsViewModel.InvoiceTaxes)
        {
            orderDetailsViewModel.Total += i.TaxAmount;
        }
        orderDetailsViewModel.Total = (double?)Math.Round((decimal)orderDetailsViewModel.Total, 2);

        return orderDetailsViewModel;
    }
    public List<InvoiceTax> GetInvoiceTaxes(int orderId, OrderDetailsViewModel orderDetailsViewModel)
    {
        List<InvoiceTax> invoiceTaxes = new List<InvoiceTax>();
        Order order = _context.Orders.Find(orderId);
        List<OrderTaxis> orderTaxes = _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToList();
        foreach (OrderTaxis orderTax in orderTaxes)
        {
            TaxesFee tax = _context.TaxesFees.Find(orderTax.TaxId);
            InvoiceTax invoiceTax = new InvoiceTax();
            if (tax.TaxType == "Percentage")
            {
                invoiceTax.TaxName = tax.Name;
                invoiceTax.TaxAmount = (double)(orderDetailsViewModel.SubTotal * (double)orderTax.TaxAmount / 100);
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
    public byte[] GenerateInvoice(int orderId)
    {

        OrderDetailsViewModel orderDetailsViewModel = GetOrderDetails(orderId);

        using (MemoryStream ms = new MemoryStream())
        {
            string htmlContent = System.IO.File.ReadAllText("Views/Order/Invoicetemplate.html");
            htmlContent = htmlContent.Replace("{{invoiceNumber}}", orderDetailsViewModel.InvoiceNumber);
            htmlContent = htmlContent.Replace("{{invoiceDate}}", orderDetailsViewModel.PlacedOn);
            htmlContent = htmlContent.Replace("{{section}}", orderDetailsViewModel.Section);
            htmlContent = htmlContent.Replace("{{customerName}}", orderDetailsViewModel.CustomerName);
            htmlContent = htmlContent.Replace("{{customerPhone}}", orderDetailsViewModel.CustomerPhone);
            string tables = "";
            foreach (DAL.Models.Table table in orderDetailsViewModel.Tables)
            {
                tables += table.Name + ",";
            }
            tables = tables.TrimEnd(',');
            htmlContent = htmlContent.Replace("{{table}}", tables);
            //for loop for invoice items
            string invoiceItems = "";
            foreach (InvoiceItem invoiceItem in orderDetailsViewModel.InvoiceItems)
            {
                string invoiceModifiers = "";
                foreach (InvoiceModifiers invoiceModifier in invoiceItem.InvoiceModifiers)
                {
                    invoiceModifiers += "<tr><td></td><td>" + invoiceModifier.Item + "</td><td>" + invoiceModifier.Quantity + "</td><td >" + invoiceModifier.Price + "</td><td style='text-align: right;'>" + invoiceModifier.TotalAmount + "</td></tr>";
                }
                invoiceItems += "<tr><td>" + invoiceItem.SrNo + "</td><td>" + invoiceItem.Item + "</td><td>" + invoiceItem.Quantity + "</td><td> " + invoiceItem.Price + "</td><td style='text-align: right;'>" + invoiceItem.TotalAmount + "</td></tr>" + invoiceModifiers;
            }
            htmlContent = htmlContent.Replace("{{invoiceItems}}", invoiceItems);
            string invoiceTaxes = "<tr><td></td><td>SubTotal</td><td></td><td></td><td style='text-align: right;'>" + orderDetailsViewModel.SubTotal + "</td></tr>";
            foreach (InvoiceTax invoiceTax in orderDetailsViewModel.InvoiceTaxes)
            {
                invoiceTaxes += "<tr><td></td><td>" + invoiceTax.TaxName + "</td><td></td><td></td><td style='text-align: right;'>" + invoiceTax.TaxAmount + "</td></tr>";
            }
            invoiceTaxes += "<tr><td></td><td>Total Amount Due</td><td></td><td></td><td style='text-align: right;'>" + orderDetailsViewModel.Total + "</td></tr>";
            htmlContent = htmlContent.Replace("{{invoiceTaxes}}", invoiceTaxes);
            htmlContent = htmlContent.Replace("{{paymentType}}", "N/A");
            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            ConverterProperties converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(htmlContent, pdf, converterProperties);
            return ms.ToArray();
        }
    }

}