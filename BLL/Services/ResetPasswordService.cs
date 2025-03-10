using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BLL.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly PizzaShopContext _context;
        public ResetPasswordService(PizzaShopContext context)
        {
            _context = context;
        }

        public User GetUserDataById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public JsonResult ResetPassword(int userId, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.SaveChanges();
                return new JsonResult(new { success = true, message = "Password reset successfully" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }
        }
    }
}