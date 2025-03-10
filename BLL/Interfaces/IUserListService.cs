using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces
{
    public interface IUserListService
    {
        public List<User> GetUsers();
        public JsonResult DeleteUser(int userId);
        public User GetUserDataById(int userId);
        public string GetRoleById(int roleId);
        public List<Role> GetRoles();
        public EditUserViewModel GetUserDataFromUserId(int userId, int userIdLoggedIn);
        public JsonResult UpdateUserDataFromUserId(int userId, EditUserViewModel userViewModel);
        public JsonResult CreateUser(int userId, CreateUserViewModel userViewModel);
        public int GetTotalUsers();
        public (List<User>, int totalRecords) GetUsers(int pageIndex, int pageSize);
        public (List<User>, int totalRecords) GetUsersWithSearch(int pageIndex, int pageSize, string searchValue, string sortColumn, string sortColumnDirection);

    }
}