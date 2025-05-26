using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ILoginService
    {
        Task<IActionResult> ValidateAsync(string email, string password);
        Task<IActionResult> Validate2FAAsync(string code);
    }
}