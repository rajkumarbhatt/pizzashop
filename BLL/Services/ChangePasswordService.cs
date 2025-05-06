using DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Implementation;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services {
    public class ChangePasswordService: IChangePasswordService {
        private readonly PizzaShopContext _context;
        public ChangePasswordService(PizzaShopContext context) {
            _context = context;
        }
        public async Task < IActionResult > ChangePasswordAsync(int userId, string newPassword, string oldPassword) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null) {
                if (BCrypt.Net.BCrypt.Verify(oldPassword, user.Password)) {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    await _context.SaveChangesAsync();
                    return new JsonResult(new {
                        success = true, message = "Password changed successfully"
                    });
                } else {
                    return new JsonResult(new {
                        success = false, message = "Old password is incorrect"
                    });
                }
            } else {
                return new JsonResult(new {
                    success = false, message = "User not found"
                });
            }
        }
    }
}