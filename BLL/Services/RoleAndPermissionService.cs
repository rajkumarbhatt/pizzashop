using System.Text.Json;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class RoleAndPermissionService : IRoleAndPermissionService
    {
        private readonly PizzaShopContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;
        private readonly INavBarService _navBarService;
        private readonly ILogger<RoleAndPermissionService> _logger;
        public RoleAndPermissionService(PizzaShopContext context, IHttpContextAccessor httpContextAccessor, IJwtService jwtService, INavBarService navBarService, ILogger<RoleAndPermissionService> logger)
        {
            _logger = logger;
            _jwtService = jwtService;
            _navBarService = navBarService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<Permission>> GetPermissionsAsync()
        {
            try
            {
                var permissions = await _context.Permissions.OrderBy(p => p.Id).ToListAsync();
                permissions.RemoveAt(8);
                permissions.RemoveAt(7);
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving permissions");
                Console.WriteLine(ex.Message);
                return new List<Permission>();
            }
        }
        public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId)
        {
            try
            {
                return await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role permissions for role ID {RoleId}", roleId);
                Console.WriteLine(ex.Message);
                return new List<RolePermission>();
            }
        }
        public async Task<Role> GetRoleAsync(int roleId)
        {
            try
            {
                return await _context.Roles.FindAsync(roleId) ?? new Role();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role with ID {RoleId}", roleId);
                Console.WriteLine(ex.Message);
                return new Role();
            }
        }
        public async Task<List<Role>> GetRolesAsync()
        {
            try
            {
                return await _context.Roles.OrderBy(r => r.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles");
                Console.WriteLine(ex.Message);
                return new List<Role>();
            }
        }
        public async Task<IActionResult> UpdateRolePermissionsAsync(List<PermissionChangeModel> changedPermissions)
        {
            try
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
                _logger.LogInformation("Permissions updated successfully for role ID {RoleId}", changedPermissions[0].RoleId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Permissions updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating permissions");
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while updating permissions",
                    error = ex.Message
                });
            }
        }
        public async Task<RoleAndPermissionViewModel> GetRoleAndPermissionViewModelAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role and permission view model");
                Console.WriteLine(ex.Message);
                return new RoleAndPermissionViewModel();
            }
        }
        public async Task<EditPermissionsViewModel> GetEditPermissionsViewModelAsync(int roleIdRequested)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving edit permissions view model");
                Console.WriteLine(ex.Message);
                return new EditPermissionsViewModel();
            }
        }
    }
}