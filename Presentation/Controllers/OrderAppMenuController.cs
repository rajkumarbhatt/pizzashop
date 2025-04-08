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
        public async Task<ActionResult> Index(int? orderId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetKotMenuAsync(orderId);
            return View(kotMenuViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetKotMenuItemsBasedOnCategory(int categoryId, string search)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.SearchMenuItemsKotAsync(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SearchMenuItemsKot(string search, int categoryId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.SearchMenuItemsKotAsync(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpPut]
        public async Task<JsonResult> AddToFavourites(int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.AddToFavouritesAsync(itemId, userId);
        }
        [HttpDelete]
        public async Task<JsonResult> DeleteFromFavourites(int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.DeleteFromFavouritesAsync(itemId, userId);
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(int orderId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetCustomerDetailsAsync(orderId);
            return PartialView("_CustomerDetailsModal", kotMenuViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerDetails([FromForm] WaitingListModal waitingListModal)
        {
            if (!ModelState.IsValid)
            {
            return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.UpdateCustomerDetailsAsync(waitingListModal, userId);
        }
        [HttpGet]
        public async Task<IActionResult> GetSelectModifiersModalData(int itemId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetSelectModifiersModalDataAsync(itemId);
            return PartialView("_SelectModifiersModal", kotMenuViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddItemToOrder(int orderId, int itemId, List<int> modifierIds)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.AddItemToOrderAsync(itemId, orderId, modifierIds, userId);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteItemFromOrder (int orderId, int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.DeleteItemFromOrderAsync(orderId, itemId, userId);
        }
    }
}