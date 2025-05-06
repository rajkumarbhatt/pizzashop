using MailKit.Net.Smtp;
using MimeKit;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using DAL.DBContext;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services {
    public class EmailService: IEmailService {
        private readonly IConfiguration _configuration;
        private readonly PizzaShopContext _context;

        public EmailService(IConfiguration configuration, PizzaShopContext context) {
            _context = context;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, User user, string resetLink) {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = "Reset your password";

            var emailTemplate = await System.IO.File.ReadAllTextAsync("Views/EmailTemplate/forgotPasswordEmail.html");
            emailTemplate = emailTemplate.Replace("{{resetLink}}", resetLink);

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
                Text = emailTemplate
            };

            using(var client = new SmtpClient()) {
                await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"] ?? ""), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendCreateUserEmailAsync(string email, string password) {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Account Created";

            var emailTemplate = await System.IO.File.ReadAllTextAsync("Views/EmailTemplate/createUserEmail.html");
            emailTemplate = emailTemplate.Replace("{{email}}", email);
            emailTemplate = emailTemplate.Replace("{{password}}", password);

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
                Text = emailTemplate
            };

            using(var client = new SmtpClient()) {
                await client.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"] ?? ""), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public async Task < IActionResult > SendForgotPasswordEmailAsync(string email) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null) {
                var userId = user.Id;
                var expiryDate = DateTime.UtcNow.AddDays(1);
                var token = userId + "_" + expiryDate.ToString();
                token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var resetLink = $"http://localhost:5125/resetpassword?token={token}";
                await SendEmailAsync(user.Email, user, resetLink);

                return new JsonResult(new {
                    success = true, message = "Email Sent"
                });
            } else {
                return new JsonResult(new {
                    success = false, message = "Email not registered"
                });
            }
        }
    }
}