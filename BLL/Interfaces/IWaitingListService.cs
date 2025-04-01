using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IWaitingListService
    {
        public WaitingListViewModel GetWaitingListViewModel();
        public IActionResult DeleteWaitingList(int id, int userId);
        public WaitingListViewModel GetWaitingListDetails(int id);
        public IActionResult GetCustomerSuggestions(string email);
        public WaitingListViewModel GetWaitingListBasedOnSection(int sectionId);
        public JsonResult GetAvailableTables(int sectionId);
        public IActionResult AssignTable(int waitingListId, int tableId, int userId, int sectionId);
    }
}