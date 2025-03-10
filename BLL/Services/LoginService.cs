using DAL.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DAL.DBContext;

namespace BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly PizzaShopContext _context;
        private readonly IJwtService _jwtService;

        public LoginService(PizzaShopContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public IActionResult Validate(string email, string password)
        {
            
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User does not exist" });
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) {
                return new JsonResult(new { success = false, message = "Invalid password" }); 
            }
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                if ((bool)user.IsDeleted)
                {
                    return new JsonResult(new { success = false, message = "User does not exist" });
                }
                if (!(bool)user.Status)
                {
                    return new JsonResult(new { success = false, message = "User is inactive" });
                }

                string? role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId).Name;
                string token = _jwtService.GenerateJwtToken(user, role);
                return new JsonResult(new { token = token, success = true, message = "Login successful" });
            }
            return new JsonResult(new { success = false, message = "Invalid credentials" });
        }
    }
}