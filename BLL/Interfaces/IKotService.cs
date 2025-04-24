using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces;

public interface IKotService
{
    Task<KotViewModel> GetKotViewModelAsync(int pageIndex, int pageSize, int? orderId = null);
    Task<KotViewModel> GetKotByCategoryAsync(int categoryId, int pageIndex, int pageSize, int? orderId = null);
    Task<KotViewModel> GetReadyItemsAsync(int categoryId, int pageIndex, int? orderId = null);
    Task<KotViewModel> GetMarkedAsPreparedModalAsync(int pageIndex, int orderId, int categoryId, bool inReady);
    Task<KotViewModel> MarkItemsAsReadyAsync(int pageIndex, List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId);
    Task<KotViewModel> MarkItemsAsInPreparedAsync(int pageIndex, List<MarkAsReadyModal> readyItems, int orderId, int categoryId, int userId);
}