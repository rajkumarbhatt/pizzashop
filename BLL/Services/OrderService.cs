using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;

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
            if (sort == "id") {
                orderTables = order == "asc" ? orderTables.OrderBy(o => o.Id).ToList() : orderTables.OrderByDescending(o => o.Id).ToList();
            }
            else if (sort == "date") {
                orderTables = order == "asc" ? orderTables.OrderBy(o => DateTime.Parse(o.Date)).ToList() : orderTables.OrderByDescending(o => DateTime.Parse(o.Date)).ToList();
            }
            else if (sort == "customerName") {
                orderTables = order == "asc" ? orderTables.OrderBy(o => o.CustomerName).ToList() : orderTables.OrderByDescending(o => o.CustomerName).ToList();
            }
            else if (sort == "totalAmount") {
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
        Order order = _context.Orders.Find(orderId);
        Customer customer = _context.Customers.Find(order.CustomerId) ?? new Customer();
        CustomerReview customerReview = _context.CustomerReviews.Where(cr => cr.CustomerId == order.CustomerId && cr.OrderId == order.Id).FirstOrDefault() ?? new CustomerReview();

        List<InvoiceItem> invoiceItems = GetInvoiceItems(orderId);
        double subTotal = 0;
        foreach (InvoiceItem invoiceItem in invoiceItems)
        {
            subTotal += invoiceItem.TotalAmount;
        }
        int invoiceId = _context.Invoices.Where(i => i.OrderId == order.Id).Select(i => i.Id).FirstOrDefault();
        DateTime invoiceCreatedOn = order.CreatedAt.HasValue ? order.CreatedAt.Value : DateTime.Now;
        TimeSpan? orderDuration = DateTime.Now - order.CreatedAt;
        string customInvoiceNumber = "INV" + invoiceCreatedOn + "-" + invoiceId;
        customInvoiceNumber = customInvoiceNumber.Replace(" ", "").Replace("-", "").Replace(":", "").Replace("/", "");
        int? tableId = _context.OrderTableMappings.Where(otm => otm.Orderid == order.Id).Select(otm => otm.Tableid).FirstOrDefault();
        int? sectionId = _context.Tables.Find(tableId).SectionId;   
        OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
        {
            Id = order.Id,
            InvoiceNumber = customInvoiceNumber,
            PaidOn = "N/A",
            ModifiedOn = order.UpdatedAt.HasValue ? order.UpdatedAt.Value.ToString() : "N/A",
            PlacedOn = order.CreatedAt.HasValue ? order.CreatedAt.Value.ToString() : "N/A",
            OrderDuration = orderDuration.HasValue ? orderDuration.Value.Days + " days " + orderDuration.Value.Hours + " hours " + orderDuration.Value.Minutes + " minutes" : "N/A",
            OrderStatus = order.Status ?? "N/A",
            CustomerName = customer.Name ?? "N/A",
            CustomerEmail = customer.Email ?? "N/A",
            CustomerPhone = customer.Phone ?? "N/A",
            NumberOfPeople = _context.OrderTableMappings.Where(otm => otm.Orderid == order.Id).Select(otm => otm.Noofpersons).FirstOrDefault().ToString() ?? "N/A",
            Table = _context.OrderTableMappings.Where(otm => otm.Orderid == order.Id).Select(otm => otm.Table).FirstOrDefault().Name ?? "N/A",
            Section = _context.Sections.Find(sectionId).Name ?? "N/A",
            InvoiceItems = invoiceItems,
            SubTotal = subTotal
        };
        return orderDetailsViewModel;
    }

    public List<InvoiceItem> GetInvoiceItems(int orderId)
    {
        List<InvoiceItem> invoiceItems = new List<InvoiceItem>();
        List<OrderItem> orderItems = _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
        foreach (OrderItem orderItem in orderItems)
        {
            Item item = _context.Items.Find(orderItem.ItemId) ?? new Item();
            List<InvoiceModifiers> invoiceModifiers = GetInvoiceModifiers(orderItem.Id);
            double price = (double)_context.Items.Find(orderItem.ItemId).Price;
            double totalAmount = orderItem.Quantity * price;
            InvoiceItem invoiceItem = new InvoiceItem
            {
                SrNo = orderItem.Id,
                Item = item.Name ?? "N/A",
                Quantity = orderItem.Quantity,
                Price = price,
                TotalAmount = totalAmount,
                InvoiceModifiers = invoiceModifiers
            };
            invoiceItems.Add(invoiceItem);
        }
        return invoiceItems;
    }

    public List<InvoiceModifiers> GetInvoiceModifiers(int invoiceItemId)
    {
        List<InvoiceModifiers> invoiceModifiers = new List<InvoiceModifiers>();
        OrderItem orderItem = _context.OrderItems.Find(invoiceItemId) ?? new OrderItem();
        List<int> modifierIds = _context.OrderModifiers.Where(om => om.OrderItemId == orderItem.Id).Select(om => om.ModifierId).ToList();
        List<Modifier> modifiers = _context.Modifiers.Where(m => modifierIds.Contains(m.Id)).ToList();
        foreach (Modifier modifier in modifiers)
        {
            InvoiceModifiers invoiceModifier = new InvoiceModifiers
            {
                Item = modifier.Name ?? "N/A",
                Quantity = 1,
                Price = (double)modifier.Price,
                TotalAmount = (double)modifier.Price
            };
            invoiceModifiers.Add(invoiceModifier);
        }
        return invoiceModifiers;
    }
}
