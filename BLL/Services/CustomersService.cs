using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using ClosedXML.Excel;

namespace BLL.Services;
public class CustomersService : ICustomerService
{
    private readonly PizzaShopContext _context;
    public CustomersService(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerTable>> GetCustomersAsync()
    {
        return await Task.Run(() => GetCustomers());
    }
    public async Task<CustomerViewModel> GetCustomerDetailsAsync()
    {
        return await Task.Run(() => GetCustomerDetails());
    }
    public async Task<CustomerViewModel> FilterCustomersAsync(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
    {
        return await Task.Run(() => FilterCustomers(pageIndex, pageSize, searchValue, time, sort, order, fromDate, toDate));
    }
    public async Task<List<CustomerTable>> GetCustomersBassedOnFiltersAsync(string time, string searchValue, string fromDate, string toDate)
    {
        return await Task.Run(() => GetCustomersBassedOnFilters(time, searchValue, fromDate, toDate));
    }
    public async Task<CustomerHistory> GetCustomerDetailsAsync(int customerId)
    {
        return await Task.Run(() => GetCustomerDetails(customerId));
    }
    public async Task<byte[]> ExportCustomersAsync(string time, string searchValue, string fromDate, string toDate)
    {
        return await Task.Run(() => ExportCustomers(time, searchValue, fromDate, toDate));
    }
    
    public List<CustomerTable> GetCustomers()
    {
        List<CustomerTable> customers = new List<CustomerTable>();
        List<Customer> customerList = _context.Customers.ToList();
        foreach (Customer customer in customerList)
        {
            int totalOrders = 0;
            List<Order> orders = _context.Orders.Where(o => o.CustomerId == customer.Id && o.IsDeleted == false).OrderByDescending(o => o.CreatedAt).ToList();
            if (orders.Count != 0)
            {
                totalOrders = orders.Count;
            }
            CustomerTable customerTable = new CustomerTable
            {
                Id = customer.Id,
                Name = customer.Name ?? "N/A",
                PhoneNumber = customer.Phone ?? "N/A",
                Email = customer.Email ?? "N/A",
                Date = customer.CreatedAt.HasValue ? customer.CreatedAt.Value.ToString("dd/MM/yyyy") : "N/A",
                TotalOrders = totalOrders
            };
            customers.Add(customerTable);
        }
        customers = customers.OrderBy(c => c.Name).ToList();
        return customers;
    }

    public CustomerViewModel GetCustomerDetails()
    {

        List<CustomerTable> customers = GetCustomers();
        CustomerViewModel customerViewModel = new CustomerViewModel
        {
            Customers = customers.Skip(0).Take(5).ToList(),
            pageIndex = 1,
            pageSize = 5,
            totalCustomers = customers.Count,
            totalPages = (int)Math.Ceiling((decimal)customers.Count / 5)
        };
        return customerViewModel;
    }

    public CustomerViewModel FilterCustomers(int pageIndex, int pageSize, string searchValue, string time, string sort, string order, string fromDate, string toDate)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;

        List<CustomerTable> customers = GetCustomers();

        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                customers = customers.Where(c => c.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
            else if (time == "Custom Date")
            {
                if (fromDate != null && toDate != null)
                {
                    customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Parse(fromDate) && DateTime.Parse(c.Date) <= DateTime.Parse(toDate)).ToList();
                }
            }
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            customers = customers.Where(c => c.Name.ToLower().Contains(searchValue)).ToList();
        }
        if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
        {
            if (sort == "name")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.Name).ToList() : customers.OrderByDescending(c => c.Name).ToList();
            }
            else if (sort == "date")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.Date).ToList() : customers.OrderByDescending(c => c.Date).ToList();
            }
            else if (sort == "totalOrders")
            {
                customers = order == "asc" ? customers.OrderBy(c => c.TotalOrders).ToList() : customers.OrderByDescending(c => c.TotalOrders).ToList();
            }
        }

        int totalCustomers = customers.Count;
        int totalPages = (int)Math.Ceiling((decimal)totalCustomers / pageSize);
        CustomerViewModel customerViewModel = new CustomerViewModel
        {
            Customers = customers.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            pageIndex = pageIndex,
            pageSize = pageSize,
            totalCustomers = totalCustomers,
            totalPages = totalPages
        };
        return customerViewModel;
    }

    public List<CustomerTable>  GetCustomersBassedOnFilters(string time, string searchValue, string fromDate, string toDate)
    {
        if (fromDate == "dd-mm-yyyy") fromDate = null;
        if (toDate == "dd-mm-yyyy") toDate = null;

        List<CustomerTable> customers = GetCustomers();

        if (!string.IsNullOrEmpty(time) && time != "All Time")
        {
            if (time == "Today")
            {
                customers = customers.Where(c => c.Date == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            }
            else if (time == "This Week")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddDays(-7)).ToList();
            }
            else if (time == "This Month")
            {
                customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Now.AddMonths(-1)).ToList();
            }
            else if (time == "Custom Date")
            {
                if (fromDate != null && toDate != null)
                {
                    customers = customers.Where(c => DateTime.Parse(c.Date) >= DateTime.Parse(fromDate) && DateTime.Parse(c.Date) <= DateTime.Parse(toDate)).ToList();
                }
            }
        }
        if (!string.IsNullOrEmpty(searchValue))
        {
            customers = customers.Where(c => c.Name.ToLower().Contains(searchValue)).ToList();
        }
        
        return customers;
    }

    public CustomerHistory GetCustomerDetails(int customerId)
    {
        Customer customer = _context.Customers.FirstOrDefault(c => c.Id == customerId) ?? new Customer();
        List<Order> orders = _context.Orders.Where(o => o.CustomerId == customerId && o.IsDeleted == false).OrderByDescending(o => o.CreatedAt).ToList();
        List<OrderItem> orderItems = _context.OrderItems.Where(oi => orders.Select(o => o.Id).Contains(oi.OrderId)).ToList();
        double avaerageBill = (double)(orders.Count != 0 ? orders.Average(o => o.TotalAmount) : 0);
        string comingSince = orders.Count != 0 ? orders.LastOrDefault().CreatedAt.Value.ToString("dd/MM/yyyy hh:mm tt") : "N/A";
        string maxOrderAmount = orders.Count != 0 ? orders.Max(o => o.TotalAmount).ToString() : "N/A";
        CustomerHistory customerHistory = new CustomerHistory
        {
            Name = customer.Name,
            PhoneNumber = customer.Phone,
            AverageBill = avaerageBill,
            ComingSince = comingSince,
            MaxOrderAmount = maxOrderAmount,
            Visits = orders.Count,
            Orders = orders.Select(o => new CustomerHistoryOrderDetails
            {
                OrderDate = o.CreatedAt.Value.ToString("dd/MM/yyyy"),
                OrderType = "Dine In",
                OrderAmount = (double?)o.TotalAmount,
                PaymentType = o.PaymentMode,
                NumberOfItems = orderItems.Where(oi => oi.OrderId == o.Id).Count()
            }).ToList()
        };
        return customerHistory;
    }

    public byte[] ExportCustomers(string time, string searchValue, string fromDate, string toDate)
    {
        var customers = GetCustomersBassedOnFilters(time, searchValue, fromDate, toDate);

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
                    return content;
                }

            }
    }
}
