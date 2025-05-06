using System.Text;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services {
    public class ResetPasswordService: IResetPasswordService {
        private readonly PizzaShopContext _context;
        public ResetPasswordService(PizzaShopContext context) {
            _context = context;
        }

        public async Task < User > GetUserDataByIdAsync(int userId) {
            return await _context.Users.FindAsync(userId) ?? new User();
        }

        public async Task < JsonResult > ResetPasswordAsync(int userId, string newPassword) {
            var user = await _context.Users.FindAsync(userId);
            if (user != null) {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.HasLoggedInBefore = true;
                user.UpdatedAt = DateTime.Now;
                user.UpdatedBy = userId;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new JsonResult(new {
                    success = true, message = "Password reset successfully"
                });
            } else {
                return new JsonResult(new {
                    success = false, message = "User not found"
                });
            }
        }

        public async Task < bool > IsLinkPresentAsync(string token) {
            return await _context.ResetPasswordLinks.AnyAsync(l => l.Link == token);
        }

        public async Task < ResetPasswordViewModel > GetResetPasswordViewModelAsync(string token) {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var tokenParts = tokenData.Split("_");
            int id = int.Parse(tokenParts[0]);
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel {
                Token = token,
                    UserId = id,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty
            };
            return await Task.FromResult(resetPasswordViewModel);
        }

        public async Task < bool > IsTokenValidAsync(string token) {
            var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var tokenParts = tokenData.Split("_");
            int id = int.Parse(tokenParts[0]);
            var expiry = tokenParts[1];
            return await Task.FromResult(DateTime.Parse(expiry) > DateTime.UtcNow);
        }

        public async Task < JsonResult > ResetPassword2Async(int userId, string newPassword, string token) {
            var user = await _context.Users.FindAsync(userId) ?? new User();
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _context.ResetPasswordLinks.Add(new ResetPasswordLink {
                Link = token,
            });
            await _context.SaveChangesAsync();
            return new JsonResult(new {
                success = true, message = "Password reset successfully"
            });
        }
    }
}