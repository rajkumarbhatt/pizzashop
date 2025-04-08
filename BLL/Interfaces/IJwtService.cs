using DAL.Models;

namespace BLL.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateJwtTokenAsync(User user, string role);
        Task<int> GetUserIdFromJwtTokenAsync(string token);
        Task SetSessionParametersAsync(int userId, string username, int roleId);
    }
}