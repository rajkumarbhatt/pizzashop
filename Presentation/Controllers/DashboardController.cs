using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using DAL.ViewModels;

namespace Presentaion.Controllers
{
    [CustomAuth]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DashboardController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly IDashboardService _dashboardService;
        public DashboardController(IJwtService jwtService, INavBarService navBarService, IDashboardService dashboardService)
        {
            _jwtService = jwtService;
            _navBarService = navBarService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["token"];
            var userId = await _jwtService.GetUserIdFromJwtTokenAsync(token??"");
            var hasLoggedInBefore = await _navBarService.IsFirstTimeLoginAsync(userId);
            var username = await _navBarService.GetUsernameFromUserIdAsync(userId);
            var roleId  = await _navBarService.GetRoleIdFromUserIdAsync(userId);
            await _jwtService.SetSessionParametersAsync(userId, username, roleId);
            if (roleId == 3)
            {
                return RedirectToAction("Index", "KOT");
            }
            if (hasLoggedInBefore == false)
            {
                return RedirectToAction("NewPassword","ResetPassword");
            }
            DashboardViewModel dashboardViewModel = await _dashboardService.GetDashboardDataAsync("Current Month");
            return View(dashboardViewModel);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("email");
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> GetUpdatedData(string TimePeriod, string? fromDate = null, string? toDate = null)        
        {
            DashboardViewModel dashboardViewModel = await _dashboardService.GetDashboardDataAsync(TimePeriod, fromDate, toDate);
            return PartialView("_DashboardPartial", dashboardViewModel);
        }
    }
}