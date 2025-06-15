namespace GBDMS.Services
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = "alerts.gbdms@gmail.com";
        public string SenderPassword { get; set; } = "rguafenybilqozpu";
        public string SenderName { get; set; } = "Gilgit-Baltistan Disaster Management System";
        public bool UseSSL { get; set; } = true;
        
        // Instructions for setting up Gmail App Password
        public static string GetGmailSetupInstructions()
        {
            return @"
To set up Gmail SMTP for this application:

1. Go to your Google Account settings (https://myaccount.google.com/)
2. Navigate to Security > 2-Step Verification (enable if not already enabled)
3. Go to Security > App passwords
4. Generate a new app password for 'Mail'
5. Use this 16-character app password (not your regular Gmail password)
6. Update the SenderPassword in EmailConfiguration

Example app password format: 'abcd efgh ijkl mnop' (remove spaces when using)

Current email: alerts.gbdms@gmail.com
Current app password: rgua feny bilq ozpu (Generated for Mail app)

If authentication still fails:
- Verify the email address exists and is accessible
- Ensure 2-factor authentication is enabled on the Gmail account
- Generate a fresh app password
- Check if 'Less secure app access' is disabled (it should be, use app passwords instead)
";
        }
    }
}
