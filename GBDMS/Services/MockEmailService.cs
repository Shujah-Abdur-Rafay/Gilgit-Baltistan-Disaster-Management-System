using System;
using System.Threading.Tasks;

namespace GBDMS.Services
{
    /// <summary>
    /// Mock email service for testing purposes when Gmail authentication is not available
    /// </summary>
    public class MockEmailService : IEmailService
    {
        public async Task SendSubscriptionConfirmationAsync(string recipientEmail, string selectedArea)
        {
            // Simulate email sending delay
            await Task.Delay(1000);
            
            var areaText = selectedArea == "all" ? "all districts" : $"{selectedArea} district";
            
            // Log the email content instead of actually sending
            Console.WriteLine("=== SUBSCRIPTION CONFIRMATION EMAIL ===");
            Console.WriteLine($"To: {recipientEmail}");
            Console.WriteLine($"Subject: Alert Subscription Confirmation - GBDMS");
            Console.WriteLine($"Content: You have successfully subscribed to disaster alert notifications for {areaText}.");
            Console.WriteLine($"Subscription Date: {DateTime.Now:dd MMM yyyy, HH:mm}");
            Console.WriteLine("========================================");
            
            // In a real scenario, this would send an actual email
            // For now, we'll just simulate success
        }

        public async Task SendAlertNotificationAsync(string recipientEmail, string alertType, string area, string severity, string message)
        {
            // Simulate email sending delay
            await Task.Delay(500);
            
            // Log the email content instead of actually sending
            Console.WriteLine("=== ALERT NOTIFICATION EMAIL ===");
            Console.WriteLine($"To: {recipientEmail}");
            Console.WriteLine($"Subject: 🚨 {severity} Alert: {alertType} in {area} - GBDMS");
            Console.WriteLine($"Alert Type: {alertType}");
            Console.WriteLine($"Severity: {severity}");
            Console.WriteLine($"Area: {area}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"Time: {DateTime.Now:dd MMM yyyy, HH:mm}");
            Console.WriteLine("=================================");
            
            // In a real scenario, this would send an actual email
            // For now, we'll just simulate success
        }
    }
}
