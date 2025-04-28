using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(string? TimePeriod, string? fromDate2 = null, string? toDate2 = null);
    DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek);

    
}