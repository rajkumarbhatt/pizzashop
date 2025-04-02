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
        public IActionResult AddToWaitingList([FromForm] WaitingListModal waitingListModal)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _orderAppService.AddToWaitingList(waitingListModal, userId);
        }
        [HttpGet]
        public JsonResult GetWaitingListForCurrentSection(int sectionId)
        {
            return _orderAppService.GetWaitingListForCurrentSection(sectionId);
        }

        [HttpPost]
        public IActionResult AssignTablesToCustomer([FromForm] WaitingListModal waitingListModal,[FromForm] string tableIds)
        {

            string sanitizedTableIds = tableIds.Replace("[", "").Replace("]", "").Trim();
            List<int> tableIdArray = sanitizedTableIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();

            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _orderAppService.AssignTablesToCustomer(waitingListModal, tableIdArray, userId);
        }
    }
}