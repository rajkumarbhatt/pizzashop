using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;

namespace BLL.Interfaces
{
    public interface IKotMenuService
    {
        public Task<KotMenuViewModel> GetKotMenuAsync(int? orderId);
        public Task<KotMenuViewModel> GetKotMenuItemsBasedOnCategoryAsync(int categoryId);
        public Task<KotMenuViewModel> SearchMenuItemsKotAsync(string search, int categoryId);
        public Task<JsonResult> AddToFavouritesAsync(int itemId, int userId);
        public Task<JsonResult> DeleteFromFavouritesAsync(int itemId, int userId);
        public Task<KotMenuViewModel> GetCustomerDetailsAsync(int orderId);
        public Task<JsonResult> UpdateCustomerDetailsAsync(WaitingListModal waitingListModal, int userId);
        public Task<KotMenuViewModel> GetSelectModifiersModalDataAsync(int itemId);
        public Task<IActionResult> AddItemToOrderAsync(int itemId, int orderId, List<int> modifierIds, int userId);
        public Task<IActionResult> DeleteItemFromOrderAsync (int orderId, int itemId, int userId);
    }
}