using System.Text.Json;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class RoleAndPermissionService : IRoleAndPermissionService
    {
        private readonly PizzaShopContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleAndPermissionService(PizzaShopContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public RoleAndPermissionService(PizzaShopContext context)
        {
            _context = context;
        }

        public List<Permission> GetPermissions()
        {
            return _context.Permissions.OrderBy(p => p.Id).ToList();
        }

        public List<RolePermission> GetRolePermissions(int roleId)
        {
            return _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToList();
        }

        public Role GetRole(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.OrderBy(r => r.Id).ToList();
        }

        public IActionResult UpdateRolePermissions(List<PermissionChangeModel> changedPermissions)
        {
            foreach (var permissionChange in changedPermissions)
            {
                RolePermission rolePermission = _context.RolePermissions.FirstOrDefault(rp =>
                    rp.RoleId == permissionChange.RoleId && rp.PermissionId == permissionChange.PermissionId);
                if (rolePermission != null)
                {
                    if (permissionChange.PermissionName == "CanView")
                    {
                        rolePermission.CanView = permissionChange.Checked;
                        _context.SaveChanges();
                    }
                    else if (permissionChange.PermissionName == "CanEdit")
                    {
                        rolePermission.CanEdit = permissionChange.Checked;
                        _context.SaveChanges();
                    }
                    else if (permissionChange.PermissionName == "CanDelete")
                    {
                        rolePermission.CanDelete = permissionChange.Checked;
                        _context.SaveChanges();
                    }

                }
            }

            if (_httpContextAccessor.HttpContext.Session.GetInt32("RoleId") == changedPermissions[0].RoleId)
            {
                List<PermissionModel> permissions = new List<PermissionModel>();
                var rolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == changedPermissions[0].RoleId).ToList();
                foreach (var rolePermission2 in rolePermissions)
                {
                    var permission = _context.Permissions.FirstOrDefault(p => p.Id == rolePermission2.PermissionId);
                    if (permission != null)
                    {
                        var permissionModel = new PermissionModel
                        {
                            PermissionId = permission.Id,
                            Name = permission.Name,
                            CanView = (bool)rolePermission2.CanView,
                            CanEdit = (bool)rolePermission2.CanEdit,
                            CanDelete = (bool)rolePermission2.CanDelete
                        };
                        permissions.Add(permissionModel);
                    }

                }
                permissions = permissions.OrderBy(p => p.PermissionId).ToList();
                byte[] permissionsBytes = JsonSerializer.SerializeToUtf8Bytes(permissions);
                _httpContextAccessor.HttpContext.Session.Set("permissions", permissionsBytes);
            }
            return new JsonResult(new { success = true, message = "Permissions updated successfully" });
        }
    }
}