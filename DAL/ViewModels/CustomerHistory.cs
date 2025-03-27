namespace DAL.ViewModels;

public class CustomerHistory
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public double? AverageBill { get; set; }
    public string? ComingSince { get; set; }
    public string? MaxOrderAmount { get; set; }
    public int? Visits { get; set; }
    public List<CustomerHistoryOrderDetails>? Orders { get; set; }
}