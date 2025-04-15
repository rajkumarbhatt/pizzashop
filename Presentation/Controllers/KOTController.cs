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
    [HttpGet]
    public async Task<IActionResult> GetKotByCategory (int categoryId)
    {
        KotViewModel kotViewModel = await _kotService.GetKotByCategoryAsync(categoryId);
        return PartialView("_CardsPartial", kotViewModel);
    }
    [HttpGet]
    public async Task<IActionResult> GetMarkedAsPreparedModal (int orderId, int categoryId)
    {
        KotViewModel kotViewModel = await _kotService.GetMarkedAsPreparedModalAsync(orderId, categoryId);
        return PartialView("_MarkedAsPreparedModal", kotViewModel);
    }
    [HttpGet]
    public async Task<IActionResult> MarkItemsAsReady (List<MarkAsReadyModal> readyItems, int orderId)
    {
        return Ok();
    }
}