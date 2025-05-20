using System.Text;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class ResetPasswordService : IResetPasswordService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<ResetPasswordService> _logger;
        public ResetPasswordService(PizzaShopContext context, ILogger<ResetPasswordService> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<User> GetUserDataByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId) ?? new User();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user data for user ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new User();
            }
        }
        public async Task<JsonResult> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    user.HasLoggedInBefore = true;
                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = userId;
                    _context.Users.Update(user);
                    _logger.LogInformation("Password reset successfully for user with ID {UserId}", userId);
                    await _context.SaveChangesAsync();
                    return new JsonResult(new
                    {
                        success = true,
                        message = "Password reset successfully"
                    });
                }
                else
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password for user with ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while resetting the password",
                    error = ex.Message
                });
            }
        }
        public async Task<bool> IsLinkPresentAsync(string token)
        {
            try
            {
                return await _context.ResetPasswordLinks.AnyAsync(l => l.Link == token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the link is present");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<ResetPasswordViewModel> GetResetPasswordViewModelAsync(string token)
        {
            try
            {
                var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var tokenParts = tokenData.Split("_");
                int id = int.Parse(tokenParts[0]);
                ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel
                {
                    Token = token,
                    UserId = id,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty
                };
                return await Task.FromResult(resetPasswordViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the reset password view model");
                Console.WriteLine(ex.Message);
                return new ResetPasswordViewModel
                {
                    Token = string.Empty,
                    NewPassword = string.Empty,
                    ConfirmPassword = string.Empty
                };
            }
        }
        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                var tokenData = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var tokenParts = tokenData.Split("_");
                int id = int.Parse(tokenParts[0]);
                var expiry = tokenParts[1];
                return await Task.FromResult(DateTime.Parse(expiry) > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the token is valid");
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<JsonResult> ResetPassword2Async(int userId, string newPassword, string token)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId) ?? new User();
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _context.ResetPasswordLinks.Add(new ResetPasswordLink
                {
                    Link = token,
                });
                await _context.SaveChangesAsync();
                _logger.LogInformation("Password reset successfully for user with ID {UserId}", userId);
                return new JsonResult(new
                {
                    success = true,
                    message = "Password reset successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password for user with ID {UserId}", userId);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while resetting the password",
                    error = ex.Message
                });
            }
        }
    }
}