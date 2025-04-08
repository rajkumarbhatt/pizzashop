using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;

namespace Presentaion.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IResetPasswordService _resetPasswordService;
        public ResetPasswordController(PizzaShopContext context, IJwtService jwtService, IResetPasswordService resetPasswordService)
        {
            _jwtService = jwtService;
            _resetPasswordService = resetPasswordService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (await _resetPasswordService.IsLinkPresentAsync(token))
            {
            return RedirectToAction("Index", "PageNotFound");
            }

            if (await _resetPasswordService.IsTokenValidAsync(token))
            {
            ResetPasswordViewModel resetPasswordViewModel = await _resetPasswordService.GetResetPasswordViewModelAsync(token);
            return View(resetPasswordViewModel);
            }
            else
            {
            return View("Index", "PageNotFound");
            }
        }

        [HttpPost]
        [Route("/api/resetpassword1")]
        public async Task<JsonResult> ResetPassword1([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
            return new JsonResult(new { success = false, message = "Validation errors" });
            }
            if (await _resetPasswordService.IsLinkPresentAsync(model.Token))
            {
            return new JsonResult(new { success = false, message = "Link already used" });
            }
            var userId = model.UserId;
            if (await _resetPasswordService.IsTokenValidAsync(model.Token))
            {
            return await _resetPasswordService.ResetPassword2Async(userId, model.NewPassword, model.Token);
            }
            else
            {
            return new JsonResult(new { success = false, message = "Token expired" });
            }
        }

        public IActionResult NewPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("/ResetPassword/NewPassword")]
        public async Task<IActionResult> NewPassword([FromBody] ResetPasswordFirstTimeViewModel model)
        {
            try
            {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Validation errors" });
            }
            var userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _resetPasswordService.ResetPasswordAsync(userId, model.NewPassword);
            }
            catch (Exception ex)
            {
            return new JsonResult(new { success = false, message = ex.Message });
            }
        }

    }
}