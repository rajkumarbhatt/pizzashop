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

        public async Task<IActionResult> Index()
        {
            OrderAppViewModel orderAppViewModel = await _orderAppService.GetOrderAppViewModelAsync();
            return View(orderAppViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWaitingList([FromForm] WaitingListModal waitingListModal)
        {
            if (!ModelState.IsValid)
            {
            return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _orderAppService.AddToWaitingListAsync(waitingListModal, userId);
        }

        [HttpGet]
        public async Task<JsonResult> GetWaitingListForCurrentSection(int sectionId)
        {
            return await _orderAppService.GetWaitingListForCurrentSectionAsync(sectionId);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTablesToCustomer([FromForm] WaitingListModal waitingListModal, [FromForm] string tableIds)
        {
            string sanitizedTableIds = tableIds.Replace("[", "").Replace("]", "").Trim();
            List<int> tableIdArray = sanitizedTableIds.Split(',').Select(id => int.Parse(id.Trim())).ToList();

            if (!ModelState.IsValid)
            {
            return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _orderAppService.AssignTablesToCustomerAsync(waitingListModal, tableIdArray, userId);
        }
    }
}