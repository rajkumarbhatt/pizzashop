using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces;

public interface IKotService
{
    Task<KotViewModel> GetKotViewModelAsync(int? orderId = null);
    Task<KotViewModel> GetKotByCategoryAsync(int categoryId, int? orderId = null);
    Task<KotViewModel> GetReadyItems(int categoryId, int? orderId = null);
    Task<KotViewModel> GetMarkedAsPreparedModalAsync(int orderId, int categoryId, bool inReady);
    Task<KotViewModel> MarkItemsAsReadyAsync(List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId);
    Task<KotViewModel> MarkItemsAsInPrepared(List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId);
}