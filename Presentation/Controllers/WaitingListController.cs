using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class WaitingListController : Controller
    {
        private readonly IWaitingListService _waitingListService;
        private readonly IJwtService _jwtService;
        public WaitingListController(IWaitingListService waitingListService, IJwtService jwtService)
        {
            _waitingListService = waitingListService;
            _jwtService = jwtService;
        }
        public ActionResult Index()
        {
            WaitingListViewModel waitingListViewModel = _waitingListService.GetWaitingListViewModel();
            return View(waitingListViewModel);
        }

        [HttpDelete]
        public IActionResult DeleteWaitingList(int id)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _waitingListService.DeleteWaitingList(id, userId);
        }

        [HttpGet]
        public IActionResult GetWaitingList()
        {
            WaitingListViewModel waitingListViewModel = _waitingListService.GetWaitingListViewModel();
            return PartialView("_WaitingListPartial", waitingListViewModel);
        }

        [HttpGet]
        public IActionResult EditWaitingList(int id)
        {
            WaitingListViewModel waitingListViewModel = _waitingListService.GetWaitingListDetails(id);
            return PartialView("_WaitingTokenModal", waitingListViewModel);
        }
        [HttpGet]
        public IActionResult GetCustomerSuggestions(string email)
        {
            return _waitingListService.GetCustomerSuggestions(email);
        }

        [HttpGet]
        public IActionResult GetWaitingListBasedOnSection(int sectionId)
        {
            WaitingListViewModel waitingListViewModel = _waitingListService.GetWaitingListBasedOnSection(sectionId);
            return PartialView("_WaitingListTablePartial", waitingListViewModel);
        }
        [HttpGet]
        public IActionResult GetAvailableTables(int sectionId)
        {
            return _waitingListService.GetAvailableTables(sectionId);
        }
        [HttpPost]
        public IActionResult AssignTable(int waitingListId, int tableId, int sectionId)
        {
            int userId = _jwtService.GetUserIdFromJwtToken(Request.Cookies["token"]);
            return _waitingListService.AssignTable(waitingListId, tableId, userId, sectionId);
        }
    }
}