using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(string? TimePeriod, string? fromDate2 = null, string? toDate2 = null);
    Task<IActionResult> EnableTwoFactorAuthenticationAsync();
    Task<IActionResult> DisableTwoFactorAuthenticationAsync();   
}