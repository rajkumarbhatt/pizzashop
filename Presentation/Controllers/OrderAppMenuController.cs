using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class OrderAppMenu : Controller
    {
        private readonly IKotMenuService _kotMenuService;
        public OrderAppMenu(IKotMenuService kotMenuService)
        {
            _kotMenuService = kotMenuService;
        }
        [Route("/OrderApp/Menu")]
        public ActionResult Index()
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetKotMenu();
            return View(kotMenuViewModel);
        }
        [HttpGet]
        public IActionResult GetKotMenuItemsBasedOnCategory(int categoryId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.GetKotMenuItemsBasedOnCategory(categoryId);
            return PartialView("_KotMenuItems", kotMenuViewModel);
        }
        [HttpGet]
        public IActionResult SearchMenuItemsKot(string search, int categoryId)
        {
            KotMenuViewModel kotMenuViewModel = _kotMenuService.SearchMenuItemsKot(search, categoryId);
            return PartialView("_KotMenuItems", kotMenuViewModel);
        }
    }
}