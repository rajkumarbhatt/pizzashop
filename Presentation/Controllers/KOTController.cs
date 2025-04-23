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
        KotViewModel kotViewModel = await _kotService.GetKotViewModelAsync(1, 4);
        return View(kotViewModel);
    }    
    [HttpGet]
    public async Task<IActionResult> GetKotByCategory (int categoryId, int pageIndex = 1, int pageSize = 4)
    {
        KotViewModel kotViewModel = await _kotService.GetKotByCategoryAsync(categoryId, pageIndex, pageSize);
        return PartialView("_CardsPartial", kotViewModel);
    }
    [HttpGet]
    public async Task<IActionResult> GetMarkedAsPreparedModal (int pageIndex, int orderId, int categoryId, bool inReady)
    {
        KotViewModel kotViewModel = await _kotService.GetMarkedAsPreparedModalAsync(pageIndex, orderId, categoryId, inReady);
        return PartialView("_MarkedAsPreparedModal", kotViewModel);
    }
    [HttpPost]
    public async Task<IActionResult> MarkItemsAsReady (int pageIndex, List<MarkAsReadyModal> readyItems, int orderId, int categoryId, bool inReady)
    {
        KotViewModel kotViewModel = new KotViewModel();
        int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
        if (inReady)
        {
            kotViewModel = await _kotService.MarkItemsAsInPrepared(pageIndex, readyItems, orderId, categoryId, userId);
        }
        else
        {
            kotViewModel = await _kotService.MarkItemsAsReadyAsync(pageIndex, readyItems, orderId, categoryId, userId);
        }
        return PartialView("_CardsPartial", kotViewModel); 
    }
    [HttpGet]
    public async Task<IActionResult> GetReadyItems (int categoryId, int pageIndex = 1)
    {
        KotViewModel kotViewModel = await _kotService.GetReadyItems(categoryId, pageIndex);
        return PartialView("_CardsPartial", kotViewModel);
    }
}