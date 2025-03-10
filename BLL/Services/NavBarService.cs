using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;

namespace BLL.Services
{
    public class NavBarService : INavBarService
    {
        private readonly PizzaShopContext _context;
        public NavBarService(PizzaShopContext context)
        {
            _context = context;
        }
                public string GetUsernameFromUserId(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                return user.Username;
            }
            return "";
        }

        public string GetProfileImageUrlFromUserId(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                return user.ProfileImage ?? "";
            }
            return "";
        }

        public int GetRoleIdFromUserId(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                return user.RoleId;
            }
            return 0;
        }

        public bool IsFirstTimeLogin(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user.HasLoggedInBefore == true)
            {
                return true;
            }
            return false;
        }

        public List<PermissionModel> GetRolePermissionsFromRoleId(int roleId)
        {
            var rolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId).OrderBy(rp => rp.PermissionId).ToList();
            var permissionModels = new List<PermissionModel>();
            foreach (var rolePermission in rolePermissions)
            {
                var permission = _context.Permissions.FirstOrDefault(p => p.Id == rolePermission.PermissionId);
                if (permission != null)
                {
                    var permissionModel = new PermissionModel
                    {
                        PermissionId = permission.Id,
                        Name = permission.Name,
                        CanView = (bool)rolePermission.CanView,
                        CanEdit = (bool)rolePermission.CanEdit,
                        CanDelete = (bool)rolePermission.CanDelete
                    };
                    permissionModels.Add(permissionModel);
                }
            }
            return permissionModels;
        }
    }
}

