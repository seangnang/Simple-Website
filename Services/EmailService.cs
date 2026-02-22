using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace SimpleWebsite.Services
{
    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            var settings = configuration.GetSection("EmailSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(settings["DisplayName"], settings["Email"]));
            email.To.Add(new MailboxAddress(toName, toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings["Host"], int.Parse(settings["Port"]!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings["Email"], settings["Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEnrollmentEmailAsync(string toEmail, string toName, string courseTitle)
        {
            var subject = $"Successfully Enrolled in {courseTitle}!";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: #4f46e5; padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>LearnHub</h1>
                    </div>
                    <div style='padding: 30px; background: #f9fafb;'>
                        <h2>Hi {toName}! 🎉</h2>
                        <p>You have successfully enrolled in <strong>{courseTitle}</strong>.</p>
                        <p>Start learning today and track your progress on your dashboard.</p>
                        <a href='#' style='background: #4f46e5; color: white; padding: 12px 24px; 
                                           text-decoration: none; border-radius: 8px; display: inline-block;'>
                            Start Learning
                        </a>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #6b7280;'>
                        <p>LearnHub — Learn Without Limits</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, toName, subject, body);
        }

        public async Task SendCompletionEmailAsync(string toEmail, string toName, string courseTitle)
        {
            var subject = $"Congratulations! You completed {courseTitle}!";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: #10b981; padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>LearnHub</h1>
                    </div>
                    <div style='padding: 30px; background: #f9fafb;'>
                        <h2>Congratulations {toName}! 🏆</h2>
                        <p>You have successfully completed <strong>{courseTitle}</strong>!</p>
                        <p>Your certificate is now available on your dashboard.</p>
                        <a href='#' style='background: #10b981; color: white; padding: 12px 24px;
                                           text-decoration: none; border-radius: 8px; display: inline-block;'>
                            View Certificate
                        </a>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #6b7280;'>
                        <p>LearnHub — Learn Without Limits</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, toName, subject, body);
        }
    }
}