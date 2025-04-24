using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(string? TimePeriod);
    DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek);

    
}