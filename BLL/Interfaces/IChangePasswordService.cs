using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IChangePasswordService
    {
        public IActionResult ChangePassword(int userId, string newPassword, string oldPassword);

    }
}