using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers
{
    [CustomAuth]
    public class TaxAndFee : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}