using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxFiling.Data;
using TaxFiling.Domain.Common;
using System.Net;
using System.Net.Mail;

namespace TaxFiling.Business.Services;

public class EmailService : IEmailService
{
    private readonly Context _context;
    private readonly ILogger<EmailService> _logger;

    public EmailService(Context context, ILogger<EmailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailRequest request)
    {
        bool isSuccessfullySent = false;
        try
        {
            var emailSetting = await _context.EmailSettings.AsNoTracking().FirstOrDefaultAsync(e => e.IsActive);
            if (emailSetting is null)
            {
                _logger.LogInformation("Email configuration not found");
                return false;
            }

            string mailerName = emailSetting.MailerName;
            string host = emailSetting.Host;
            int port = emailSetting.Port;
            string fromEmail = emailSetting.FromEmail;
            string username = emailSetting.UserName;
            string password = emailSetting.Password;

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(username, password)                
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, mailerName),
                Subject = request.Subject,
                Body = request.BodyHtml,
                IsBodyHtml = true
            };

            // Add TO recipients
            foreach (var to in request.ToEmails ?? Enumerable.Empty<string>())
            {
                mailMessage.To.Add(to);
            }

            // Add CC recipients
            foreach (var cc in request.CcList ?? Enumerable.Empty<string>())
            {
                mailMessage.CC.Add(cc);
            }

            // Add attachments
            if (request.Attachments != null)
            {
                foreach (var attachment in request.Attachments)
                {
                    var bytes = Convert.FromBase64String(attachment.Base64Data);
                    var stream = new MemoryStream(bytes);
                    mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                }
            }

            await smtpClient.SendMailAsync(mailMessage);
            isSuccessfullySent = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending email");
        }

        return isSuccessfullySent;
    }
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailRequest request);
}
