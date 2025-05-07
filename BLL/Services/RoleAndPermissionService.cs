using System.Text.Json;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class RoleAndPermissionService : IRoleAndPermissionService
    {
        private readonly PizzaShopContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;

        public RoleAndPermissionService(PizzaShopContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, INavBarService navBarService)
        {
            _jwtService = jwtService;
            _navBarService = navBarService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Permission>> GetPermissionsAsync()
        {
            var permissions = await _context.Permissions.OrderBy(p => p.Id).ToListAsync();
            permissions.RemoveAt(8);
            permissions.RemoveAt(7);
            return permissions;
        }

        public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        }

        public async Task<Role> GetRoleAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId) ?? new Role();
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.OrderBy(r => r.Id).ToListAsync();
        }

        public async Task<IActionResult> UpdateRolePermissionsAsync(List<PermissionChangeModel> changedPermissions)
        {
            if (changedPermissions.Count == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No permissions to update"
                });
            }

            foreach (var permissionChange in changedPermissions)
            {
                var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(rp =>
                    rp.RoleId == permissionChange.RoleId && rp.PermissionId == permissionChange.PermissionId) ?? new RolePermission();

                if (rolePermission != null)
                {
                    if (permissionChange.PermissionName == "CanView")
                    {
                        rolePermission.CanView = permissionChange.Checked;
                        await _context.SaveChangesAsync();
                    }
                    else if (permissionChange.PermissionName == "CanEdit")
                    {
                        rolePermission.CanEdit = permissionChange.Checked;
                        await _context.SaveChangesAsync();
                    }
                    else if (permissionChange.PermissionName == "CanDelete")
                    {
                        rolePermission.CanDelete = permissionChange.Checked;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            if (_httpContextAccessor.HttpContext?.Session.GetInt32("RoleId") == changedPermissions[0].RoleId)
            {
                var permissions = new List<PermissionModel>();
                var rolePermissions = await _context.RolePermissions.Where(rp => rp.RoleId == changedPermissions[0].RoleId).ToListAsync();

                foreach (var rolePermission2 in rolePermissions)
                {
                    var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == rolePermission2.PermissionId);
                    if (permission != null)
                    {
                        var permissionModel = new PermissionModel
                        {
                            PermissionId = permission.Id,
                            Name = permission.Name,
                            CanView = rolePermission2.CanView ?? false,
                            CanEdit = rolePermission2.CanEdit ?? false,
                            CanDelete = rolePermission2.CanDelete ?? false
                        };
                        permissions.Add(permissionModel);
                    }
                }

                permissions = permissions.OrderBy(p => p.PermissionId).ToList();
                byte[] permissionsBytes = JsonSerializer.SerializeToUtf8Bytes(permissions);
                _httpContextAccessor.HttpContext.Session.Set("permissions", permissionsBytes);
            }

            return new JsonResult(new
            {
                success = true,
                message = "Permissions updated successfully"
            });
        }

        public async Task<RoleAndPermissionViewModel> GetRoleAndPermissionViewModelAsync()
        {
            var userId = await _jwtService.GetUserIdFromJwtTokenAsync(_httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? "");
            var username = await _navBarService.GetUsernameFromUserIdAsync(userId);
            var profileImageURL = await _navBarService.GetProfileImageUrlFromUserIdAsync(userId);
            var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
            var roles = await GetRolesAsync();
            var permissions = await _navBarService.GetRolePermissionsFromRoleIdAsync(roleId);

            var roleAndPermissionViewModel = new RoleAndPermissionViewModel
            {
                Username = username,
                ProfileImageURL = profileImageURL,
                RoleId = roleId,
                Roles = roles,
                Permissions = permissions
            };

            return roleAndPermissionViewModel;
        }

        public async Task<EditPermissionsViewModel> GetEditPermissionsViewModelAsync(int roleIdRequested)
        {
            var userId = await _jwtService.GetUserIdFromJwtTokenAsync(_httpContextAccessor.HttpContext?.Request.Cookies["token"] ?? "");
            var roleId = await _navBarService.GetRoleIdFromUserIdAsync(userId);
            var permission = await GetPermissionsAsync();
            var roleRequested = await GetRoleAsync(roleIdRequested);
            var rolePermissions = await GetRolePermissionsAsync(roleIdRequested);
            var permissions = await _navBarService.GetRolePermissionsFromRoleIdAsync(roleId);
            permission.RemoveAll(p => p.Name == "RoleAndPermission");
            var editPermissionsViewModel = new EditPermissionsViewModel
            {
                Permission = permission,
                RolePermissions = rolePermissions,
                RequestedRole = roleRequested,
                Permissions = permissions
            };

            return editPermissionsViewModel;
        }
    }
}