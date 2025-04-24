using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL.ViewModels;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserListService
    {
        public Task<List<User>> GetUsersAsync();
        public Task<JsonResult> DeleteUserAsync(int userId);
        public Task<User> GetUserDataByIdAsync(int userId);
        public Task<string> GetRoleByIdAsync(int roleId);
        public Task<List<Role>> GetRolesAsync(int userId);
        public Task<EditUserViewModel> GetUserDataFromUserIdAsync(int userId, int userIdLoggedIn);
        public Task<JsonResult> UpdateUserDataFromUserIdAsync(int userId, EditUserViewModel userViewModel);
        public Task<JsonResult> CreateUserAsync(int userId, CreateUserViewModel userViewModel);
        public Task<int> GetTotalUsersAsync();
        public Task<(List<User>, int totalRecords)> GetUsersAsync(int pageIndex, int pageSize, int userId);
        public Task<(List<User>, int totalRecords)> GetUsersWithSearchAsync(int pageIndex, int pageSize, string searchValue, string sortColumn, string sortColumnDirection);
        public Task<UserListViewModel> GetUsersListViewModelAsync(int pageIndex, int pageSize, int userId);
        public Task<UserListViewModel> GetUsersListViewModelSearchAsync(int pageIndex, int pageSize, string sortColumn, string sortColumnDirection, string searchValue);
        public Task<CreateUserViewModel> GetCreateUserViewModelAsync();
    }
}