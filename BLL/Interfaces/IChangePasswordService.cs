using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IChangePasswordService
    {
        public Task<IActionResult> ChangePasswordAsync(int userId, string newPassword, string oldPassword);

    }
}