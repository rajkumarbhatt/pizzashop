using DAL.Models;

namespace DAL.ViewModels
{
    public class RoleAndPermissionViewModel
    {
        public string Username { get; set; }
        public string ProfileImageURL { get; set; }
        public int RoleId { get; set; }
        public List<Role> Roles { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
}