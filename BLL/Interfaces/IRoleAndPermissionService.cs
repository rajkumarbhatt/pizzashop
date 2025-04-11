using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IRoleAndPermissionService
    {
        Task<List<Permission>> GetPermissionsAsync();
        Task<List<RolePermission>> GetRolePermissionsAsync(int roleId);
        Task<Role> GetRoleAsync(int roleId);
        Task<List<Role>> GetRolesAsync();
        Task<IActionResult> UpdateRolePermissionsAsync(List<PermissionChangeModel> changedPermissions);
        Task<RoleAndPermissionViewModel> GetRoleAndPermissionViewModelAsync();
        Task<EditPermissionsViewModel> GetEditPermissionsViewModelAsync(int roleIdRequested);
    }
}