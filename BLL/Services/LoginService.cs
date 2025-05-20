using DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly PizzaShopContext _context;
        private readonly ILogger<LoginService> _logger;
        private readonly IJwtService _jwtService;
        public LoginService(PizzaShopContext context, IJwtService jwtService, ILogger<LoginService> logger)
        {
            _logger = logger;
            _context = context;
            _jwtService = jwtService;
        }
        public async Task<IActionResult> ValidateAsync(string email, string password)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} does not exist", email);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "User does not exist"
                    });
                }
                if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    _logger.LogWarning("Invalid password for user with email {Email}", email);
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Invalid password"
                    });
                }
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    if (user.IsDeleted ?? false)
                    {
                        _logger.LogWarning("User with email {Email} is deleted", email);
                        return new JsonResult(new
                        {
                            success = false,
                            message = "User does not exist"
                        });
                    }
                    if (!user.Status ?? false)
                    {
                        _logger.LogWarning("User with email {Email} is inactive", email);
                        return new JsonResult(new
                        {
                            success = false,
                            message = "User is inactive"
                        });
                    }

                    var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                    if (roleEntity == null)
                    {
                        return new JsonResult(new
                        {
                            success = false,
                            message = "User role not found"
                        });
                    }
                    string role = roleEntity.Name;
                    string token = await _jwtService.GenerateJwtTokenAsync(user, role);
                    _logger.LogInformation("User with email {Email} logged in successfully", email);
                    return new JsonResult(new
                    {
                        token = token,
                        success = true,
                        message = "Login successful"
                    });
                }
                _logger.LogWarning("Invalid credentials for user with email {Email}", email);
                return new JsonResult(new
                {
                    success = false,
                    message = "Invalid credentials"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating the user with email {Email}", email);
                Console.WriteLine(ex.Message);
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while validating the user",
                    error = ex.Message
                });
            }
        }
    }
}