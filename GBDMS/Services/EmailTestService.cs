using System;
using System.Threading.Tasks;

namespace GBDMS.Services
{
    /// <summary>
    /// Simple test service to verify email functionality
    /// </summary>
    public class EmailTestService
    {
        private readonly IEmailService _emailService;

        public EmailTestService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> TestEmailConnection()
        {
            try
            {
                Console.WriteLine("🧪 Testing email connection...");
                Console.WriteLine("📋 Current configuration:");
                Console.WriteLine("   Email: alerts.gbdms@gmail.com");
                Console.WriteLine("   App Password: rgua feny bilq ozpu (Mail)");
                Console.WriteLine("   SMTP: smtp.gmail.com:587");
                Console.WriteLine("");

                // Test with a simple subscription confirmation
                await _emailService.SendSubscriptionConfirmationAsync(
                    "test@example.com",
                    "Gilgit"
                );

                Console.WriteLine("✅ Email test successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email test failed: {ex.Message}");
                Console.WriteLine("");
                Console.WriteLine("🔧 Troubleshooting steps:");
                Console.WriteLine("1. Verify 2FA is enabled on Gmail account");
                Console.WriteLine("2. Generate a new App Password specifically for 'GBDMS'");
                Console.WriteLine("3. Make sure 'Less secure app access' is OFF");
                Console.WriteLine("4. Try the new App Password in the configuration");
                return false;
            }
        }

        public async Task<bool> TestAlertNotification()
        {
            try
            {
                Console.WriteLine("🧪 Testing alert notification...");
                
                await _emailService.SendAlertNotificationAsync(
                    "test@example.com",
                    "Avalanche",
                    "Gilgit",
                    "Warning",
                    "Test alert message for email verification"
                );
                
                Console.WriteLine("✅ Alert notification test successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Alert notification test failed: {ex.Message}");
                return false;
            }
        }
    }
}
