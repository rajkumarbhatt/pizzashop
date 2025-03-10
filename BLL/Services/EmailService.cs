using MailKit.Net.Smtp;
using MimeKit;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Configuration;


namespace BLL.Services
{


    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, User user, string resetLink)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Reset your password";

            var emailTemplate = System.IO.File.ReadAllText("Views/EmailTemplate/forgotPasswordEmail.html");
            emailTemplate = emailTemplate.Replace("{{resetLink}}", resetLink);

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailTemplate
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendCreateUserEmailAsync(string email, string password)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Account Created";

            var emailTemplate = System.IO.File.ReadAllText("Views/EmailTemplate/createUserEmail.html");
            emailTemplate = emailTemplate.Replace("{{email}}", email);
            emailTemplate = emailTemplate.Replace("{{password}}", password);

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailTemplate
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}