using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IOrderAppService
    {
        public OrderAppViewModel GetOrderAppViewModel();
        public IActionResult AddToWaitingList(WaitingListModal waitingListModal, int userId);
        public JsonResult GetWaitingListForCurrentSection(int sectionId);
        public IActionResult AssignTablesToCustomer(WaitingListModal waitingListModal, List<int> tableIds, int userId);
    }
}