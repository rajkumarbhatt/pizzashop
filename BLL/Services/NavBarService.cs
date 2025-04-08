using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class NavBarService : INavBarService
    {
        private readonly PizzaShopContext _context;
        public NavBarService(PizzaShopContext context)
        {
            _context = context;
        }
        public async Task<string> GetUsernameFromUserIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
            return user.Username;
            }
            return "";
        }

        public async Task<string> GetProfileImageUrlFromUserIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
            return user.ProfileImage ?? "";
            }
            return "";
        }

        public async Task<int> GetRoleIdFromUserIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
            return user.RoleId;
            }
            return 0;
        }

        public async Task<bool> IsFirstTimeLoginAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user?.HasLoggedInBefore == true)
            {
            return true;
            }
            return false;
        }

        public async Task<List<PermissionModel>> GetRolePermissionsFromRoleIdAsync(int roleId)
        {
            var rolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .OrderBy(rp => rp.PermissionId)
            .ToListAsync();

            var permissionModels = new List<PermissionModel>();
            foreach (var rolePermission in rolePermissions)
            {
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == rolePermission.PermissionId);
            if (permission != null)
            {
                var permissionModel = new PermissionModel
                {
                PermissionId = permission.Id,
                Name = permission.Name,
                CanView = rolePermission.CanView ?? false,
                CanEdit = rolePermission.CanEdit ?? false,
                CanDelete = rolePermission.CanDelete ?? false,
                };
                permissionModels.Add(permissionModel);
            }
            }
            return permissionModels;
        }
    }
}

