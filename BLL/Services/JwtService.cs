using DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BLL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INavBarService _navBarService;
        public JwtService(IHttpContextAccessor httpContextAccessor, INavBarService navBarService)
        {
            _navBarService = navBarService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GenerateJwtTokenAsync(User user, string role)
        {
            return await Task.Run(() =>
            {
            List<Claim> claims = new List<Claim>
            {
                // Subject (sub) claim with the user's ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test1232133454353533636gfhgfhxfdsfsdfsdfghgfhfghfghgfhfghfhfgh"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5125",
                audience: "http://localhost:5125",
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }
        public async Task<int> GetUserIdFromJwtTokenAsync(string token)
        {
            return await Task.Run(() =>
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                throw new ArgumentException("Invalid JWT token");
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                throw new ArgumentException("JWT token does not contain a user ID");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new ArgumentException("Invalid user ID in JWT token");
            }

            return userId;
            });
        }
        public async Task<string> GetUsernameFromJwtTokenAsync(string token)
        {
            return await Task.Run(() =>
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                throw new ArgumentException("Invalid JWT token");
            }

            var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (usernameClaim == null)
            {
                throw new ArgumentException("JWT token does not contain a username");
            }

            return usernameClaim.Value;
            });
        }
        public async Task SetSessionParametersAsync(int userId, string username, int roleId) {
            var profileImageURL = await _navBarService.GetProfileImageUrlFromUserIdAsync(userId);
            var permissions = await _navBarService.GetRolePermissionsFromRoleIdAsync(roleId);
            var permissionsBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(permissions);
            _httpContextAccessor.HttpContext?.Session.Set("permissions", permissionsBytes);
            _httpContextAccessor.HttpContext?.Session.SetString("Username", username);
            _httpContextAccessor.HttpContext?.Session.SetString("ProfileImageURL", profileImageURL);
            _httpContextAccessor.HttpContext?.Session.SetInt32("RoleId", roleId);
            _httpContextAccessor.HttpContext?.Session.SetInt32("UserId", userId); 
        }
    }
}