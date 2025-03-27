namespace DAL.ViewModels;

public class CustomerViewModel
{
    public required List<CustomerTable> Customers;
    public int pageIndex { get; set; }
    public int pageSize { get; set; }
    public int totalCustomers { get; set; }
    public int totalPages { get; set; } 
    public CustomerHistory? CustomerHistory { get; set; }
}