using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace first_api_backend
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("mailcluster.loopia.se", 587)
            {
                Credentials = new NetworkCredential("petter.lindh@skoglit.se", "z>M5xN?>uagS%Mvm3EK9ZjM"),
                EnableSsl = true // TLS måste vara aktiverat för port 587
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("petter.lindh@skoglit.se"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}