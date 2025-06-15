using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Threading.Tasks;

namespace GBDMS.Services
{
    public interface IEmailService
    {
        Task SendSubscriptionConfirmationAsync(string recipientEmail, string selectedArea);
        Task SendAlertNotificationAsync(string recipientEmail, string alertType, string area, string severity, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _senderEmail = "alerts.gbdms@gmail.com";
        private readonly string _senderPassword = "rguafenybilqozpu"; // Gmail App Password for Mail
        private readonly string _senderName = "Gilgit-Baltistan Disaster Management System";

        public async Task SendSubscriptionConfirmationAsync(string recipientEmail, string selectedArea)
        {
            var subject = "Alert Subscription Confirmation - GBDMS";
            var areaText = selectedArea == "all" ? "all districts" : $"{selectedArea} district";
            
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f8f9fa; }}
        .footer {{ padding: 15px; text-align: center; font-size: 12px; color: #666; }}
        .alert-box {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; margin: 15px 0; border-radius: 5px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>🚨 GBDMS Alert Subscription</h2>
        </div>
        <div class='content'>
            <h3>Subscription Confirmed!</h3>
            <p>Dear Subscriber,</p>
            <p>You have successfully subscribed to disaster alert notifications for <strong>{areaText}</strong>.</p>
            
            <div class='alert-box'>
                <strong>📧 Subscription Details:</strong><br>
                • Email: {recipientEmail}<br>
                • Coverage Area: {areaText}<br>
                • Subscription Date: {DateTime.Now:dd MMM yyyy, HH:mm}<br>
            </div>
            
            <p>You will receive timely notifications about:</p>
            <ul>
                <li>🌊 Flood warnings</li>
                <li>🌍 Earthquake notifications</li>
                <li>🪨 Landslide warnings</li>
                <li>❄️ GLOF (Glacial Lake Outburst Flood) alerts</li>
            </ul>
            
            <p>Stay safe and prepared!</p>
            
            <p>Best regards,<br>
            <strong>Gilgit-Baltistan Disaster Management System</strong></p>
        </div>
        <div class='footer'>
            This is an automated message from GBDMS. Please do not reply to this email.
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(recipientEmail, subject, body);
        }

        public async Task SendAlertNotificationAsync(string recipientEmail, string alertType, string area, string severity, string message)
        {
            var subject = $"🚨 {severity} Alert: {alertType} in {area} - GBDMS";
            
            var severityColor = severity switch
            {
                "Emergency" => "#dc3545",
                "Warning" => "#ffc107",
                "Watch" => "#17a2b8",
                "Advisory" => "#6c757d",
                _ => "#28a745"
            };

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: {severityColor}; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f8f9fa; }}
        .footer {{ padding: 15px; text-align: center; font-size: 12px; color: #666; }}
        .alert-box {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; margin: 15px 0; border-radius: 5px; }}
        .emergency {{ background-color: #f8d7da; border-color: #f5c6cb; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>🚨 {severity} Alert</h2>
            <h3>{alertType} - {area}</h3>
        </div>
        <div class='content'>
            <div class='alert-box {(severity == "Emergency" ? "emergency" : "")}'>
                <strong>⚠️ Alert Details:</strong><br>
                • Type: {alertType}<br>
                • Severity: {severity}<br>
                • Area: {area}<br>
                • Time: {DateTime.Now:dd MMM yyyy, HH:mm}<br>
            </div>
            
            <h4>Message:</h4>
            <p><strong>{message}</strong></p>
            
            <h4>Recommended Actions:</h4>
            <ul>
                <li>Stay alert and monitor local conditions</li>
                <li>Follow instructions from local authorities</li>
                <li>Keep emergency supplies ready</li>
                <li>Stay tuned for further updates</li>
            </ul>
            
            <p><strong>Stay safe and take necessary precautions!</strong></p>
            
            <p>Best regards,<br>
            <strong>Gilgit-Baltistan Disaster Management System</strong></p>
        </div>
        <div class='footer'>
            This is an automated alert from GBDMS. For emergency assistance, contact local authorities immediately.
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(recipientEmail, subject, body);
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
        {
            try
            {
                Console.WriteLine($"🔧 Attempting to send email to: {recipientEmail}");
                Console.WriteLine($"📧 Using email: {_senderEmail}");
                Console.WriteLine($"🔑 App password length: {_senderPassword.Length} characters");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_senderName, _senderEmail));
                message.To.Add(new MailboxAddress("", recipientEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Enable detailed logging for debugging
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                Console.WriteLine($"🌐 Connecting to {_smtpServer}:{_smtpPort}...");

                // Try different connection methods
                try
                {
                    // Method 1: StartTLS (most common)
                    await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                }
                catch (Exception)
                {
                    Console.WriteLine("⚠️ StartTLS failed, trying SSL...");
                    // Method 2: Direct SSL
                    await client.ConnectAsync(_smtpServer, 465, SecureSocketOptions.SslOnConnect);
                }

                Console.WriteLine("✅ Connected to SMTP server");
                Console.WriteLine($"🔐 Authenticating with {_senderEmail}...");

                // Authenticate with the app password
                await client.AuthenticateAsync(_senderEmail, _senderPassword);

                Console.WriteLine("✅ Authentication successful");

                // Send the message
                await client.SendAsync(message);
                Console.WriteLine("✅ Email sent successfully");

                // Disconnect
                await client.DisconnectAsync(true);

                Console.WriteLine($"📧 Email sent successfully to {recipientEmail}");
            }
            catch (MailKit.Security.AuthenticationException authEx)
            {
                Console.WriteLine($"❌ Authentication failed: {authEx.Message}");
                Console.WriteLine($"🔍 Check these Gmail settings:");
                Console.WriteLine($"   1. 2-Factor Authentication is enabled");
                Console.WriteLine($"   2. App Password is correctly generated");
                Console.WriteLine($"   3. Email address is correct: {_senderEmail}");
                throw new Exception($"Email authentication failed. Please check the email credentials. Details: {authEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
                Console.WriteLine($"🔍 Full error details: {ex}");
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }
}
