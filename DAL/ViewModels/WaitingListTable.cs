namespace DAL.ViewModels;

public class WaitingListTable
{
    public int TokenNumber { get; set; }
    public string? CreatedAt { get; set; }
    public string? WaitingTime { get; set; }
    public string? Name { get; set; }
    public int NumberOfPersons { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}