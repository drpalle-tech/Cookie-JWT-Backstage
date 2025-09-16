using Identity.SMTP;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Identity.Services
{
    public class EmailService : IEmailService
    {
        private IOptions<SMTPModel> _smtpOptions;
        public EmailService(IOptions<SMTPModel> smptpOptions) {
            _smtpOptions = smptpOptions;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage(_smtpOptions.Value.User, to, subject, body);

            using (var emailClient = new SmtpClient(_smtpOptions.Value.Host, _smtpOptions.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(
                    _smtpOptions.Value.User, _smtpOptions.Value.Password);
                emailClient.EnableSsl = true;
                await emailClient.SendMailAsync(message);
            }
        }
    }
}
