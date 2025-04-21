using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class DashboardController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        public DashboardController(IJwtService jwtService, INavBarService navBarService)
        {
            _jwtService = jwtService;
            _navBarService = navBarService;
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
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("email");
            return RedirectToAction("Index", "Home");
        }
    }
}