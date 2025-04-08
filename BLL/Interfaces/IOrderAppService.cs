using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IOrderAppService
    {
        public Task<OrderAppViewModel> GetOrderAppViewModelAsync();
        public Task<IActionResult> AddToWaitingListAsync(WaitingListModal waitingListModal, int userId);
        public Task<JsonResult> GetWaitingListForCurrentSectionAsync(int sectionId);
        public Task<IActionResult> AssignTablesToCustomerAsync(WaitingListModal waitingListModal, List<int> tableIds, int userId);
    }
}