using DAL.Models;

namespace DAL.ViewModels
{
    public class UserListViewModel
    {
        public IEnumerable<User>? Users { get; set; }
        public string? Username { get; set; }
        public string? ProfileImageURL { get; set; }
        public int RoleId { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalUsers { get; set; } 
        public List<PermissionModel>? Permissions { get; set; }
    }
}
