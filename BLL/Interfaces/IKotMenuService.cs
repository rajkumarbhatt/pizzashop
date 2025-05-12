using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;

namespace BLL.Interfaces
{
    public interface IKotMenuService
    {
        public Task<string> GetOrderStatusAsync(int orderId);
        public Task<KotMenuViewModel> GetKotMenuAsync(int? orderId);
        public Task<KotMenuViewModel> GetKotMenuItemsBasedOnCategoryAsync(int categoryId);
        public Task<KotMenuViewModel> SearchMenuItemsKotAsync(string search, int categoryId);
        public Task<JsonResult> AddToFavouritesAsync(int itemId, int userId);
        public Task<JsonResult> DeleteFromFavouritesAsync(int itemId, int userId);
        public Task<KotMenuViewModel> GetCustomerDetailsAsync(int orderId);
        public Task<JsonResult> UpdateCustomerDetailsAsync(WaitingListModal waitingListModal, int userId);
        public Task<KotMenuViewModel> GetSelectModifiersModalDataAsync(int itemId);
        public Task<IActionResult> UpdateOrderAmountAsync (int orderId, int userId, float subTotal, float total);
        public Task<JsonResult> GetOrderWiseCommentAsync (int orderId);
        public Task<JsonResult> GetItemWiseCommentAsync(int orderItemId);
        public Task<IActionResult> AddOrderWiseCommentAsync (int orderId, string comment, int userId);
        public Task<IActionResult> AddItemWiseCommentAsync (int orderItemId, string comment, int userId);
        public Task<IActionResult> SaveOrderAsync (SaveOrderViewModel saveOrderViewModel, int userId);
        public Task<IActionResult> CompleteOrderAsync (int orderId, int userId);
        public Task<IActionResult> CancelOrderAsync (int orderId, int userId);
        public Task<IActionResult> SaveCustomerReviewAsync (SaveCustomerReviewViewModel saveCustomerReviewViewModel, int userId);
        public Task<JsonResult> CanDeleteFromOrderAsync (int orderItemId);
        public Task<JsonResult> CanReduceFromOrderAsync (int orderItemId, int currentQuantity);
        public Task<JsonResult> AreModifiersSelectedAsync (int itemId);
        // public Task<PaymentViewModal> GetPaymentViewModalAsync (int orderId);
    }
}