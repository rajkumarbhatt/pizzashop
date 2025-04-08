using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IWaitingListService
    {
        Task<WaitingListViewModel> GetWaitingListViewModelAsync();
        Task<IActionResult> DeleteWaitingListAsync(int id, int userId);
        Task<WaitingListViewModel> GetWaitingListDetailsAsync(int id);
        Task<IActionResult> GetCustomerSuggestionsAsync(string email);
        Task<WaitingListViewModel> GetWaitingListBasedOnSectionAsync(int sectionId);
        Task<JsonResult> GetAvailableTablesAsync(int sectionId);
        Task<IActionResult> AssignTableAsync(int waitingListId, int tableId, int userId, int sectionId);
    }
}