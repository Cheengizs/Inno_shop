using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Users.Application.Services.Interfaces;

namespace Users.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var fromEmail = _configuration["Gmail:Email"];
        var appPassword = _configuration["Gmail:AppPassword"];

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(fromEmail, appPassword)
        };

        var mailMessage = new MailMessage(fromEmail, to, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}