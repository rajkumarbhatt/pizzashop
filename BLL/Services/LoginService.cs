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

                List<RolePermission> rolePermissions = _context.RolePermissions.Where(rp => rp.RoleId == user.RoleId).ToList();

                // array of permission names
                List<Permission> permissions = new List<Permission>();

                foreach (RolePermission rolePermission in rolePermissions)
                {
                    Permission permission = _context.Permissions.FirstOrDefault(p => p.Id == rolePermission.PermissionId);
                    permissions.Add(permission);
                }

                // store permissions in JWT

                string? role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId).Name;
                string token = _jwtService.GenerateJwtToken(user, role, permissions);
                return new JsonResult(new { token = token, success = true, message = "Login successful" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Invalid email or password" });
            }
        }
    }
}