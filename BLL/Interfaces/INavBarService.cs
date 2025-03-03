using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces
{
    public interface INavBarService
    {
        public string GetUsernameFromUserId(int userId);
        public string GetProfileImageUrlFromUserId(int userId);
        public int GetRoleIdFromUserId(int userId);
        public bool IsFirstTimeLogin(int userId);
        public List<PermissionModel> GetRolePermissionsFromRoleId(int roleId);
    }
}