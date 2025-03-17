using Microsoft.AspNetCore.Mvc;

namespace BLL.Interfaces
{
    public interface ILoginService
    {
        public IActionResult Validate(string email, string password);
    }
}