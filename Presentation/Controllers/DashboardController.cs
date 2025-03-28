using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
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

        public IActionResult Index()
        {
            var token = Request.Cookies["token"];
            var userId = _jwtService.GetUserIdFromJwtToken(token??"");
            var hasLoggedInBefore = _navBarService.IsFirstTimeLogin(userId);
            var username = _navBarService.GetUsernameFromUserId(userId);
            var roleId  = _navBarService.GetRoleIdFromUserId(userId);
            if (roleId == 3)
            {
                return RedirectToAction("Index", "OrderApp");
            }
            if (hasLoggedInBefore == false)
            {
                return RedirectToAction("NewPassword","ResetPassword");
            }
            var profileImageURL = _navBarService.GetProfileImageUrlFromUserId(userId);
            var permissions = _navBarService.GetRolePermissionsFromRoleId(roleId);
            var permissionsBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(permissions);
            HttpContext.Session.Set("permissions", permissionsBytes);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("ProfileImageURL", profileImageURL);
            HttpContext.Session.SetInt32("RoleId", roleId);
            HttpContext.Session.SetInt32("UserId", userId); 
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