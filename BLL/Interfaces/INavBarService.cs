using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces
{
    public interface INavBarService
    {
        public Task<string> GetUsernameFromUserIdAsync(int userId);
        public Task<string> GetProfileImageUrlFromUserIdAsync(int userId);
        public Task<int> GetRoleIdFromUserIdAsync(int userId);
        public Task<bool> IsFirstTimeLoginAsync(int userId);
        public Task<bool> IsTwoFactorAuthenticationEnabledAsync(int userId);
        public Task<List<PermissionModel>> GetRolePermissionsFromRoleIdAsync(int roleId);
    }
}