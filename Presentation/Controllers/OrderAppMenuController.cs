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
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetKotMenu();
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
    }
}