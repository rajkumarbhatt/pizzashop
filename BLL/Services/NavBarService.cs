using DAL.Models;
using DAL.ViewModels;
using BLL.Interfaces;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class NavBarService : INavBarService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<NavBarService> _logger;
        public NavBarService(PizzaShopContext context, ILogger<NavBarService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<string> GetUsernameFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    return user.Username;
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the username for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public async Task<string> GetProfileImageUrlFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    return user.ProfileImage ?? "";
                }
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the profile image URL for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public async Task<int> GetRoleIdFromUserIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    return user.RoleId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the role ID for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<bool> IsFirstTimeLoginAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user?.HasLoggedInBefore == true)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user ID {UserId} is first time login", userId);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<PermissionModel>> GetRolePermissionsFromRoleIdAsync(int roleId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role permissions for role ID {RoleId}", roleId);
                Console.WriteLine(ex.Message);
                return new List<PermissionModel>();
            }
        }
    }
}