using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces;

public interface IKotService
{
    Task<KotViewModel> GetKotViewModelAsync();
    Task<KotViewModel> GetKotByCategoryAsync(int categoryId);
    Task<KotViewModel> GetMarkedAsPreparedModalAsync(int orderId, int categoryId);
}