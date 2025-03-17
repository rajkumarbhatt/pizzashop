using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class PageNotFoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}