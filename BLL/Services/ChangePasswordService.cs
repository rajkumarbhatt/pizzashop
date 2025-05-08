using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<ChangePasswordService> _logger;

        public ChangePasswordService(PizzaShopContext context, ILogger<ChangePasswordService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> ChangePasswordAsync(int userId, string newPassword, string oldPassword)
        {
            try
            {
                _logger.LogInformation("Attempting to change password for user with ID {UserId}", userId);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("Password changed successfully for user with ID {UserId}", userId);

                        return new JsonResult(new
                        {
                            success = true,
                            message = "Password changed successfully"
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Old password is incorrect for user with ID {UserId}", userId);

                        return new JsonResult(new
                        {
                            success = false,
                            message = "Old password is incorrect"
                        });
                    }
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
                _logger.LogError(ex, "An error occurred while changing the password for user with ID {UserId}", userId);

                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while changing the password",
                    error = ex.Message
                });
            }
        }
    }
}