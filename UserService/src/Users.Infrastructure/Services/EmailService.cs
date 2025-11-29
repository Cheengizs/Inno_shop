using MailKit;
using Microsoft.Extensions.Configuration;
using Users.Application.Services.Interfaces;
using MimeKit;  
using MailKit.Security;

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
        var appPassword = _configuration["Gmail:AppPassword"]?.Replace(" ", "");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("InnoShop", fromEmail));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;
        
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody 
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        try 
        {
            await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            
            await client.AuthenticateAsync(fromEmail, appPassword);
            
            await client.SendAsync(emailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SMTP Error: {ex.Message}");
            throw; 
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}