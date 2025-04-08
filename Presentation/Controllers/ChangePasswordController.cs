using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;


namespace Presentaion.Controllers
{
    [CustomAuth]
    public class ChangePasswordController : Controller
    {
        private readonly IChangePasswordService _changePasswordService;
        private readonly INavBarService _navBarService;
        private readonly IJwtService _jwtService;
        public ChangePasswordController(IChangePasswordService changePasswordService, INavBarService navBarService, IJwtService jwtService)
        {
            _changePasswordService = changePasswordService;
            _jwtService = jwtService;
            _navBarService = navBarService;
        }
        public IActionResult Index()
        {
            var token = Request.Cookies["token"];
            if (token == null)
            {
                return BadRequest("Token is missing.");
            }
            return View();
        }

        [HttpPost]
        [Route("/account/changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation errors" });
            }
            var token = Request.Cookies["token"];
            var userId = await _jwtService.GetUserIdFromJwtTokenAsync(token ?? "");
            return await _changePasswordService.ChangePasswordAsync(userId, model.NewPassword ?? "", model.CurrentPassword ?? "");
        }
    }
}