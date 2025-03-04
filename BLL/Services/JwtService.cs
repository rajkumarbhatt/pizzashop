using DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BLL.Interfaces;

namespace BLL.Services
{
    public class JwtService : IJwtService
    {
        public string GenerateJwtToken(User user, string role, List<Permission> permissions)
        {

            List<Claim> claims = new List<Claim>
            {
                // Subject (sub) claim with the user's ID
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim (ClaimTypes.Role, user.RoleId.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test1232133454353533636gfhgfhxfdsfsdfsdfghgfhfghfghgfhfghfhfgh"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5125",
                audience: "http://localhost:5125",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int GetUserIdFromJwtToken(string token)
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
        }

        public string GetUsernameFromJwtToken(string token)
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
        }

        public List<string> GetPermissionsFromJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                throw new ArgumentException("Invalid JWT token");
            }

            var permissionsClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "permissions");
            if (permissionsClaim == null)
            {
                throw new ArgumentException("JWT token does not contain permissions");
            }

            return permissionsClaim.Value.Split(',').ToList();
        }
    }
}