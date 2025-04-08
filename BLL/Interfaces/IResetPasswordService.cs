using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IResetPasswordService
    {
        Task<User> GetUserDataByIdAsync(int userId);
        Task<JsonResult> ResetPasswordAsync(int userId, string newPassword);
        Task<JsonResult> ResetPassword2Async(int userId, string newPassword, string token);
        Task<bool> IsLinkPresentAsync(string token);
        Task<ResetPasswordViewModel> GetResetPasswordViewModelAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
    }
}
