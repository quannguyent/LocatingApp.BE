using LocatingApp.Common;
using LocatingApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace LocatingApp.Services.MMail
{
    public interface IMailService : IServiceScoped
    {
        Task<Mail> SendMail(Mail Mail);
    }
    public class MailService : IMailService
    {
        private static MailSettings MailSettings;
        public MailService()
        {
            var mailConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build().GetSection("EmailConfig");
            MailSettings = new MailSettings
            {
                Mail = mailConfig["From"],
                DisplayName = mailConfig["UserName"],
                Password = mailConfig["Password"],
                Host = mailConfig["SmtpServer"],
                Port = int.Parse(mailConfig["Port"])
            };
        }
        public enum ErrorCode
        {
            MailNotSend
        }

        public async Task<Mail> SendMail(Mail Mail)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(MailSettings.DisplayName, MailSettings.Mail);
            email.From.Add(new MailboxAddress(MailSettings.DisplayName, MailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(Mail.Recipients));
            email.Subject = Mail.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = Mail.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(MailSettings.Host, MailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(MailSettings.Mail, MailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Mail.AddError(nameof(MailService), nameof(Mail), ErrorCode.MailNotSend);
            }
            smtp.Disconnect(true);
            return Mail;
        }
    }
}
