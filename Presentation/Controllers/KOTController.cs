using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class KOTController : Controller
{
    private readonly IKotService _kotService;
    private readonly IJwtService _jwtService;
    public KOTController(IKotService kotService, IJwtService jwtService)
    {
        _kotService = kotService;
        _jwtService = jwtService;
    }
    [Route("OrderApp/Kot")]
    public async Task<IActionResult> Index()
    {
        KotViewModel kotViewModel = await _kotService.GetKotViewModelAsync();
        return View(kotViewModel);
    }    
    [HttpGet]
    public async Task<IActionResult> GetKotByCategory (int categoryId)
    {
        KotViewModel kotViewModel = await _kotService.GetKotByCategoryAsync(categoryId);
        return PartialView("_CardsPartial", kotViewModel);
    }
    [HttpGet]
    public async Task<IActionResult> GetMarkedAsPreparedModal (int orderId, int categoryId, bool inReady)
    {
        KotViewModel kotViewModel = await _kotService.GetMarkedAsPreparedModalAsync(orderId, categoryId, inReady);
        return PartialView("_MarkedAsPreparedModal", kotViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> MarkItemsAsReady (List<MarkAsReadyModal> readyItems, int orderId, int categoryId, bool inReady)
    {
        KotViewModel kotViewModel = new KotViewModel();
        int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
        if (inReady)
        {
            kotViewModel = await _kotService.MarkItemsAsInPrepared(readyItems, orderId, categoryId, userId);
        }
        else
        {
            kotViewModel = await _kotService.MarkItemsAsReadyAsync(readyItems, orderId, categoryId, userId);
        }
        return PartialView("_CardsPartial", kotViewModel); 
    }
    [HttpGet]
    public async Task<IActionResult> GetReadyItems (int categoryId)
    {
        KotViewModel kotViewModel = await _kotService.GetReadyItems(categoryId);
        return PartialView("_CardsPartial", kotViewModel);
    }
}