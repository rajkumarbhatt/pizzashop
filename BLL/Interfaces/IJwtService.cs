using DAL.Models;

namespace BLL.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(User user, string role);

        public int GetUserIdFromJwtToken(string token);
    }
}