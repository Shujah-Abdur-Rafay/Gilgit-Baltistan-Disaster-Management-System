using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

class Program
{
    private static readonly string _senderEmail = "alerts.gbdms@gmail.com";
    private static readonly string _senderPassword = "rguafenybilqozpu"; // Gmail App Password for Mail
    private static readonly string _senderName = "GBDMS Alert System";
    private static readonly string _smtpServer = "smtp.gmail.com";
    private static readonly int _smtpPort = 587;

    static async Task Main(string[] args)
    {
        Console.WriteLine("🧪 GBDMS Email Test");
        Console.WriteLine("==================");
        Console.WriteLine($"📧 Email: {_senderEmail}");
        Console.WriteLine($"🔑 App Password: {_senderPassword}");
        Console.WriteLine($"🌐 SMTP: {_smtpServer}:{_smtpPort}");
        Console.WriteLine();

        try
        {
            Console.WriteLine("🔄 Testing email connection...");
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            message.To.Add(new MailboxAddress("Test User", "test@example.com"));
            message.Subject = "🧪 GBDMS Email Test - Subscription Confirmation";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = @"
                    <h2>🎉 GBDMS Email Test Successful!</h2>
                    <p>This is a test email from the GBDMS Alert System.</p>
                    <p><strong>Test Details:</strong></p>
                    <ul>
                        <li>✅ Gmail SMTP Connection: Working</li>
                        <li>✅ App Password Authentication: Working</li>
                        <li>✅ Email Sending: Working</li>
                    </ul>
                    <p>Your GBDMS email notification system is ready!</p>
                    <hr>
                    <p><small>Gilgit-Baltistan Disaster Management System</small></p>
                "
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            Console.WriteLine($"🌐 Connecting to {_smtpServer}:{_smtpPort}...");
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            Console.WriteLine("✅ Connected to SMTP server");

            Console.WriteLine($"🔐 Authenticating with {_senderEmail}...");
            await client.AuthenticateAsync(_senderEmail, _senderPassword);
            Console.WriteLine("✅ Authentication successful");

            Console.WriteLine("📤 Sending test email...");
            await client.SendAsync(message);
            Console.WriteLine("✅ Email sent successfully");

            await client.DisconnectAsync(true);
            Console.WriteLine("🔌 Disconnected from SMTP server");

            Console.WriteLine();
            Console.WriteLine("🎉 EMAIL TEST COMPLETED SUCCESSFULLY!");
            Console.WriteLine("✅ Your GBDMS email system is working perfectly!");
            Console.WriteLine("✅ Gmail App Password is valid");
            Console.WriteLine("✅ SMTP connection is working");
            Console.WriteLine();
            Console.WriteLine("📧 Note: The test email was sent to 'test@example.com'");
            Console.WriteLine("   In the real app, emails will be sent to actual user addresses.");
        }
        catch (MailKit.Security.AuthenticationException authEx)
        {
            Console.WriteLine($"❌ Authentication failed: {authEx.Message}");
            Console.WriteLine();
            Console.WriteLine("🔧 Troubleshooting steps:");
            Console.WriteLine("1. Verify 2-Factor Authentication is enabled on Gmail");
            Console.WriteLine("2. Generate a new App Password for 'Mail' application");
            Console.WriteLine("3. Make sure 'Less secure app access' is OFF");
            Console.WriteLine("4. Update the App Password in the code");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"🔍 Full error: {ex}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
