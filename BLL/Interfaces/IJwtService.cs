using DAL.Models;

namespace BLL.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(User user, string role, List<Permission> permissions);

        public int GetUserIdFromJwtToken(string token);

        // get permissions from token
        public List<string> GetPermissionsFromJwtToken(string token);
    }
}