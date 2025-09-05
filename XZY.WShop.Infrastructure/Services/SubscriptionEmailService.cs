using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Interfaces.Services;

namespace XZY.WShop.Infrastructure.Services
{
    public class SubscriptionEmailService : ISubscriptionEmailService
    {
        private readonly IEmailService _emailSender;
        private readonly ILogger<SubscriptionEmailService> _logger;
        private readonly string _baseUrl;

        public SubscriptionEmailService(IEmailService emailSender,
                                      ILogger<SubscriptionEmailService> logger,
                                      IConfiguration configuration)
        {
            _emailSender = emailSender;
            _logger = logger;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        public async Task SendSubscriptionExpiryWarningAsync(string userEmail, string userName, DateTime expiryDate, int daysUntilExpiry)
        {
            var subject = daysUntilExpiry switch
            {
                1 => "Your subscription expires tomorrow!",
                3 => "Your subscription expires in 3 days",
                7 => "Your subscription expires in 7 days",
                _ => $"Your subscription expires in {daysUntilExpiry} days"
            };

            var body = BuildExpiryWarningEmail(userName, expiryDate, daysUntilExpiry);

            try
            {
                await _emailSender.SendMailAsync(userEmail, subject, body);
                _logger.LogInformation("Expiry warning email sent to {Email}", userEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send expiry warning email to {Email}", userEmail);
            }
        }

        public async Task SendSubscriptionExpiredAsync(string userEmail, string userName, DateTime expiryDate)
        {
            var subject = "Your subscription has expired";
            var body = BuildExpiredEmail(userName, expiryDate);

            try
            {
                await _emailSender.SendMailAsync(userEmail, subject, body);
                _logger.LogInformation("Expired subscription email sent to {Email}", userEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send expired subscription email to {Email}", userEmail);
            }
        }

        private string BuildExpiryWarningEmail(string userName, DateTime expiryDate, int daysUntilExpiry)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f8f9fa; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #fff; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; 
                 color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #6c757d; font-size: 14px; }}
        .warning {{ color: #dc3545; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Subscription Expiry Notice</h1>
        </div>
        
        <div class='content'>
            <h2>Hello {userName},</h2>
            
            <p>This is a friendly reminder that your subscription will <span class='warning'>expire in {daysUntilExpiry} day{(daysUntilExpiry > 1 ? "s" : "")}</span>.</p>
            
            <p><strong>Expiry Date:</strong> {expiryDate:MMMM dd, yyyy}</p>
            
            <p>To continue enjoying uninterrupted access to all our features, 
            please renew your subscription before it expires.</p>
            
            <div style='text-align: center;'>
                <a href='{_baseUrl}/subscription/renew' class='button'>Renew Your Subscription</a>
            </div>
            
            <p>If you have any questions or need assistance, please don't hesitate to 
            contact our support team at support@yourcompany.com.</p>
            
            <p>Thank you for being a valued customer!</p>
            
            <p>Best regards,<br>The Team at YourCompany</p>
        </div>
        
        <div class='footer'>
            <p>© {DateTime.Now.Year} {ApplicationContants.AppName}. All rights reserved.</p>
            <p>
            <a href='{_baseUrl}/privacy'>Privacy Policy</a></p>
        </div>
    </div>
</body>
</html>";
        }

        private string BuildExpiredEmail(string userName, DateTime expiryDate)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #fff3cd; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #fff; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #28a745; 
                 color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .urgent {{ color: #dc3545; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⚠️ Subscription Expired</h1>
        </div>
        
        <div class='content'>
            <h2>Dear {userName},</h2>
            
            <p>We're writing to inform you that your subscription <span class='urgent'>expired on {expiryDate:MMMM dd, yyyy}</span>.</p>
            
            <p>Your access to premium features has been temporarily suspended. To restore full access:</p>
            
            <div style='text-align: center;'>
                <a href='{_baseUrl}/subscription/renew' class='button'>Reactivate Your Subscription</a>
            </div>
            
            <p><strong>What happens next?</strong></p>
            <ul>
                <li>Immediate restoration of all features upon renewal</li>
                <li>Your settings and data are preserved</li>
                <li>No setup required - pick up right where you left off</li>
            </ul>
            
            <p>If you believe this is an error or need assistance, please contact our 
            support team at {ApplicationContants.SupportEmail} or reply to this email.</p>
            
            <p>We hope to see you back soon!</p>
            
            <p>Best regards,<br>The Team at {ApplicationContants.AppName}</p>
        </div>
        
        <div class='footer'>
            <p>© {DateTime.Now.Year} {ApplicationContants.AppName}. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
