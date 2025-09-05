
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using XYZ.WShop.Application.Interfaces.Services;

namespace XZY.WShop.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        //public async Task SendMailAsync(string to, string subject, string html, string from = null)
        //{
        //    try
        //    {
        //        var client = new MailjetClient(_config["MailJetSettings:MJ_APIKEY_PUBLIC"], _config["MailJetSettings:MJ_APIKEY_PRIVATE"]);

        //        var request = new MailjetRequest
        //        {
        //            Resource = Send.Resource,
        //        }
        //        .Property(Send.FromEmail, "noreply@wakawithus.com") 
        //        .Property(Send.FromName, "WakaWithUs Team")    
        //        .Property(Send.Subject, subject)                  
        //        .Property(Send.TextPart, html)                      
        //        .Property(Send.HtmlPart, $"<p>{html}</p>")         
        //        .Property(Send.Recipients, new JArray
        //        {
        //        new JObject
        //        {
        //            {"Email", to} 
        //        }
        //        });

        //        var response = await client.PostAsync(request);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine("Email sent successfully!");
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Failed to send email. Status: {response.StatusCode}, Error: {response.GetData()}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //    }

        //}

        public async Task SendMailAsync(string to, string subject, string html, string from = null)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _config["AppSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            using var smtp = new SmtpClient();

            try
            {
                var host = _config["AppSettings:SmtpHost"];
                var port = int.Parse(_config["AppSettings:SmtpPort"]);

                await smtp.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);

                Console.WriteLine("Connected successfully, authenticating...");

                await smtp.AuthenticateAsync(
                    _config["AppSettings:SmtpUser"],
                    _config["AppSettings:SmtpPass"]);

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true);
            }
        }
    }
}
