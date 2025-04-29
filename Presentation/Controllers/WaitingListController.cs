using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class WaitingListController : Controller
    {
        private readonly IWaitingListService _waitingListService;
        private readonly IJwtService _jwtService;
        public WaitingListController(IWaitingListService waitingListService, IJwtService jwtService)
        {
            _waitingListService = waitingListService;
            _jwtService = jwtService;
        }
        public async Task<ActionResult> Index()
        {
            WaitingListViewModel waitingListViewModel = await _waitingListService.GetWaitingListViewModelAsync();
            return View(waitingListViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWaitingList(int id)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _waitingListService.DeleteWaitingListAsync(id, userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingList()
        {
            WaitingListViewModel waitingListViewModel = await _waitingListService.GetWaitingListViewModelAsync();
            return PartialView("_WaitingListPartial", waitingListViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditWaitingList(int id)
        {
            WaitingListViewModel waitingListViewModel = await _waitingListService.GetWaitingListDetailsAsync(id);
            return PartialView("_WaitingTokenModal", waitingListViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerSuggestions(string email)
        {
            return await _waitingListService.GetCustomerSuggestionsAsync(email);
        }

        [HttpGet]
        public async Task<IActionResult> GetWaitingListBasedOnSection(int sectionId)
        {
            WaitingListViewModel waitingListViewModel = await _waitingListService.GetWaitingListBasedOnSectionAsync(sectionId);
            return PartialView("_WaitingListTablePartial", waitingListViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTables(int sectionId)
        {
            return await _waitingListService.GetAvailableTablesAsync(sectionId);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTable(int waitingListId, List<int> tableIds, int sectionId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"]);
            return await _waitingListService.AssignTableAsync(waitingListId, tableIds, userId, sectionId);
        }
    }
}