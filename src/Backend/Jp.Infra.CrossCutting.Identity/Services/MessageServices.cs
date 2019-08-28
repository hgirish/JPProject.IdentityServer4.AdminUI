using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace Jp.Infra.CrossCutting.Identity.Services
{

    public class AuthEmailMessageSender : IEmailSender
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public AuthEmailMessageSender(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if(!_emailConfiguration.SendEmail)
                return;
            
            var mimeMessage = new MimeMessage();
            mimeMessage.To.Add(new MailboxAddress(email));
            mimeMessage.From.Add(new MailboxAddress(_emailConfiguration.FromName, _emailConfiguration.FromAddress));

            mimeMessage.Subject = subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            mimeMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };
            if (_emailConfiguration.SmtpDeliveryMethod == (int)System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory)
            {
                SendToPickupDirectory(mimeMessage, _emailConfiguration.PickupDirectory);
            }
            //Be careful that the SmtpClient class is the one from Mailkit not the framework!
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, _emailConfiguration.UseSsl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                
                await client.SendAsync(mimeMessage);
                client.Disconnect(true);
            }

        }
        void SendToPickupDirectory(MimeMessage message, string pickupDirectory)
        {
            do
            {
                var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");

                if (File.Exists(path))
                    continue;

                try
                {
                    using (var stream = new FileStream(path, FileMode.CreateNew))
                    {
                        message.WriteTo(stream);
                        return;
                    }
                }
                catch (IOException)
                {
                    // The file may have been created between our File.Exists() check and
                    // our attempt to create the stream.
                }
            } while (true);
        }
    }

    public class AuthSMSMessageSender : ISmsSender
    {
        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
