using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DAL.ViewModels
{
    public interface IRoleAndPermissionService
    {
        public List<Permission> GetPermissions();
        public List<RolePermission> GetRolePermissions(int roleId);
        public Role GetRole(int roleId);
        public List<Role> GetRoles();
        public IActionResult UpdateRolePermissions(List<PermissionChangeModel> changedPermissions);
    }
}