using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using DAL.DBContext;
using System.Text;

namespace Presentaion.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;
        private readonly IResetPasswordService _resetPasswordService;

        public ResetPasswordController(PizzaShopContext context, IJwtService jwtService, IResetPasswordService resetPasswordService)
        {
            _context = context;
            _jwtService = jwtService;
            _resetPasswordService = resetPasswordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("resetpassword")]
        public IActionResult ResetPassword(string token)
        {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var tokenParts = tokenData.Split("_");
            var id = tokenParts[0];
            var expiry = tokenParts[1];
            if (DateTime.Parse(expiry) > DateTime.UtcNow)
            {
                ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel
                {
                    Token = token,
                    UserId = int.Parse(id),
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty
                };
                resetPasswordViewModel.Token = token;
                resetPasswordViewModel.UserId = int.Parse(id);
                return View(resetPasswordViewModel);
            }
            else
            {
                return View("Index", "PageNotFound"); 
            }
        }

        [HttpPost]
        [Route("/api/resetpassword")]
        public JsonResult ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            try
            {
                var userId = model.UserId;
                var user = _resetPasswordService.GetUserDataById(userId);
                if (user != null)
                {
                    var token = model.Token;
                    var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var tokenParts = tokenData.Split("_");
                    var id = tokenParts[0];
                    var expiry = tokenParts[1];
                    if (DateTime.Parse(expiry) > DateTime.UtcNow)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        _context.SaveChanges();
                        return new JsonResult(new { success = true, message = "Password reset successfully" });
                    }
                    else
                    {
                        return new JsonResult(new { success = false, message = "Token expired" });
                    }
                }
                else
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
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
                var userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    user.HasLoggedInBefore = true;
                    await _context.SaveChangesAsync();
                    return new JsonResult(new { success = true, message = "Password reset successfully" });
                }
                else
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

    }
}