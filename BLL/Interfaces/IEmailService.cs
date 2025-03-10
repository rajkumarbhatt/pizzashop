using DAL.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace BLL.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, User user, string resetLink);
        public Task SendCreateUserEmailAsync(string email, string password);
    }
}