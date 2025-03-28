using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers
{
    [CustomAuth]
    public class OrderAppController : Controller
    {
        private readonly IOrderAppService _orderAppService;
        private readonly IJwtService _jwtService;

        public OrderAppController(IOrderAppService orderAppService, IJwtService jwtService)
        {
            _orderAppService = orderAppService;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            OrderAppViewModel orderAppViewModel = _orderAppService.GetOrderAppViewModel();
            return View(orderAppViewModel);
        }

        [HttpPost]
        public IActionResult AddToWaitingList (string email, string name, string mobileNumber, string sectionId, string numberOfPeople)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _orderAppService.AddToWaitingList(email, name, mobileNumber, sectionId, numberOfPeople, userId);
        }
    }
}