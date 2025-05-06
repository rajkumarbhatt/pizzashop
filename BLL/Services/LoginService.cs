using DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services {
    public class LoginService: ILoginService {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;

        public LoginService(PizzaShopContext context, IJwtService jwtService) {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task < IActionResult > ValidateAsync(string email, string password) {
            User ? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) {
                return new JsonResult(new {
                    success = false, message = "User does not exist"
                });
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) {
                return new JsonResult(new {
                    success = false, message = "Invalid password"
                });
            }
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) {
                if (user.IsDeleted ?? false) {
                    return new JsonResult(new {
                        success = false, message = "User does not exist"
                    });
                }
                if (!user.Status ?? false) {
                    return new JsonResult(new {
                        success = false, message = "User is inactive"
                    });
                }

                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                if (roleEntity == null) {
                    return new JsonResult(new {
                        success = false, message = "User role not found"
                    });
                }
                string role = roleEntity.Name;
                string token = await _jwtService.GenerateJwtTokenAsync(user, role);
                return new JsonResult(new {
                    token = token, success = true, message = "Login successful"
                });
            }
            return new JsonResult(new {
                success = false, message = "Invalid credentials"
            });
        }
    }
}