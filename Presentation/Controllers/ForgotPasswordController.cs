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