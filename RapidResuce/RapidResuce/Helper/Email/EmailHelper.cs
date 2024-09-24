using Microsoft.Extensions.Options;
using MimeKit;
using RapidResuce.Models;
using MailKit.Net.Smtp;
using RapidResuce.Interfaces;

namespace RapidResuce.Helper.Email
{
    public class EmailHelper : IEmailHelper
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailHelper(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void Send(string toEmail, string subject, string body, bool isHtmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = isHtmlBody
                        ? new TextPart("html") { Text = body }
                        : new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpSettings.Host, _smtpSettings.Port, false);
                client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
