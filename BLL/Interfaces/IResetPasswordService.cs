using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface IResetPasswordService
    {
        public User GetUserDataById(int userId);
        public JsonResult ResetPassword(int userId, string newPassword);

    }
}