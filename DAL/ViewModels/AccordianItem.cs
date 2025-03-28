namespace DAL.ViewModels;

public class AccordianItem
{
    public List<TableCard>? TableCards { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? NumberOfAvailableTables { get; set; }
    public int? NumberOfAssignedTables { get; set; }
    public int? NumberOfRunningTables { get; set; }
    public int? NumberOfSelectedTables { get; set; }
}