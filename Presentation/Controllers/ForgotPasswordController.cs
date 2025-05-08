using Microsoft.AspNetCore.Mvc;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;


namespace Presentaion.Controllers
{

    public class ForgotPasswordController : Controller
    {
        private readonly IEmailService _emailService;

        public ForgotPasswordController(IEmailService emailService, PizzaShopContext context)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            string token = Request.Cookies["token"] ?? "";
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [Route("api/forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel forgotPasswordModel)
        {
            return await _emailService.SendForgotPasswordEmailAsync(forgotPasswordModel.Email ?? "");
        }
    }
}