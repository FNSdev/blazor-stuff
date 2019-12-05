using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;

namespace hephaestus.Services
{
    public class MailingService
    {
        private readonly IConfiguration _config;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly CancellationToken _cancellationToken;

        public MailingService(IConfiguration config, IBackgroundTaskQueue taskQueue, IHostApplicationLifetime applicationLifetime)
        {
            _config = config;
            _backgroundTaskQueue = taskQueue;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void Send(string to, string subject, string body)
        {
            _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                var message = new MimeMessage()
                {
                    Sender = new MailboxAddress("Uladzislau Stasheuski", "hephaestus.noreply@gmail.com"),
                    Subject = subject,
                };
                message.Body = new TextPart(TextFormat.Html) { Text = body};
                message.To.Add(new MailboxAddress(to));
            
                using (var emailClient = new SmtpClient())
                {
                    await emailClient.ConnectAsync("smtp.gmail.com", 465, true);
                
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                
                    await emailClient.AuthenticateAsync(_config["Gmail:Email"], _config["Gmail:Password"]);
                    await emailClient.SendAsync(message);
                    await emailClient.DisconnectAsync(true);
                }                
            });
        }
    }
}
