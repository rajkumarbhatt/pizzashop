using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class KOTController : Controller
{
    private readonly IKotService _kotService;
    public KOTController(IKotService kotService)
    {
        _kotService = kotService;
    }
    [Route("OrderApp/Kot")]
    public async Task<IActionResult> Index()
    {
        KotViewModel kotViewModel = await _kotService.GetKotViewModelAsync();
        return View(kotViewModel);
    }    
}