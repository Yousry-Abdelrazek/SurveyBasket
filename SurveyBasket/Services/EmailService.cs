using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog.Core;
using SurveyBasket.Settings;

namespace SurveyBasket.Services;

// After install Mailkit 
public class EmailService(IOptions<MailSettings> mailsettings , ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailsettings = mailsettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = new MailboxAddress(_mailsettings.DisplayName, _mailsettings.Mail),
            Subject = subject

        };
        
        message.To.Add(new MailboxAddress("reciever",email));


        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using ( var smtp = new SmtpClient())
        {

            _logger.LogInformation("Sending email to {Email}", email);
            smtp.Connect(_mailsettings.Host, _mailsettings.Port, MailKit.Security.SecureSocketOptions.StartTls); //  MailKit.Security.SecureSocketOptions.StartTls useful for secure connection
            smtp.Authenticate(_mailsettings.Mail, _mailsettings.Password);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }

    }
}
