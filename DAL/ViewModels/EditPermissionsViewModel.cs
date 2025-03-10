using System.Security.Permissions;
using DAL.Models;

namespace DAL.ViewModels
{
    public class EditPermissionsViewModel
    {
        public string Username { get; set; }
        public string ProfileImageURL { get; set; }
        public int RoleId { get; set; }
        public Role RequestedRole { get; set; }
        public List<Permission> Permission { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
}