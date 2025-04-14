using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IKotService
{
    Task<KotViewModel> GetKotViewModelAsync();
}