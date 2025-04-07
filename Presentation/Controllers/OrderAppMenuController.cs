using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class OrderAppMenu : Controller
    {
        private readonly IKotMenuService _kotMenuService;
        private readonly IJwtService _jwtService;
        public OrderAppMenu(IKotMenuService kotMenuService, IJwtService jwtService)
        {
            _kotMenuService = kotMenuService;
            _jwtService = jwtService;
        }
        [Route("/OrderApp/Menu")]
        [Route("/OrderApp/Menu/{orderId}")]
        public ActionResult Index(int? orderId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetKotMenu(orderId);
            return View(kotMenuViewModel);
        }
        [HttpGet]
        public IActionResult GetKotMenuItemsBasedOnCategory(int categoryId, string search)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.SearchMenuItemsKot(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpGet]
        public IActionResult SearchMenuItemsKot(string search, int categoryId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.SearchMenuItemsKot(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpPut]
        public JsonResult AddToFavourites (int itemId) {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _kotMenuService.AddToFavourites(itemId, userId);
        }
        [HttpDelete]
        public JsonResult DeleteFromFavourites (int itemId) {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _kotMenuService.DeleteFromFavourites(itemId, userId);
        }
        [HttpGet]
        public IActionResult GetCustomerDetails(int orderId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetCustomerDetails(orderId);
            return PartialView("_CustomerDetailsModal", kotMenuViewModel);
        }
        [HttpPost]
        public IActionResult UpdateCustomerDetails ([FromForm]WaitingListModal waitingListModal)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _kotMenuService.UpdateCustomerDetails(waitingListModal, userId);
        }
        [HttpGet]
        public IActionResult GetSelectModifiersModalData(int itemId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetSelectModifiersModalData(itemId);
            return PartialView("_SelectModifiersModal", kotMenuViewModel);
        }
        [HttpPost]
        public IActionResult AddItemToOrder (int orderId, int itemId, List<int> modifierIds)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"] ?? "");
            return _kotMenuService.AddItemToOrder(itemId, orderId, modifierIds, userId);
        }
    }
}