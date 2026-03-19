<<<<<<< HEAD
﻿using SimpleWebsite.Data;
using SimpleWebsite.Models;
=======
﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f

namespace SimpleWebsite.Services
{
    public class EmailService
    {
        private readonly IConfiguration configuration;
<<<<<<< HEAD
        private readonly AppDbContext context;

        public EmailService(IConfiguration configuration, AppDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        // Save to inbox database
        public async Task SaveToInboxAsync(string userId, string subject, string body)
        {
            var log = new EmailLog
            {
                UserId = userId,
                Subject = subject,
                Body = body,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };
            context.EmailLogs.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task SendEnrollmentEmailAsync(string toEmail, string toName,
            string courseTitle, string userId)
=======

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
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
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
<<<<<<< HEAD
                        <a href='/Course/MyCourses' style='background: #4f46e5; color: white; padding: 12px 24px;
=======
                        <a href='#' style='background: #4f46e5; color: white; padding: 12px 24px; 
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
                                           text-decoration: none; border-radius: 8px; display: inline-block;'>
                            Start Learning
                        </a>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #6b7280;'>
                        <p>LearnHub — Learn Without Limits</p>
                    </div>
                </div>";

<<<<<<< HEAD
            await SaveToInboxAsync(userId, subject, body);
        }

        public async Task SendCompletionEmailAsync(string toEmail, string toName,
            string courseTitle, string userId)
=======
            await SendEmailAsync(toEmail, toName, subject, body);
        }

        public async Task SendCompletionEmailAsync(string toEmail, string toName, string courseTitle)
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
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
<<<<<<< HEAD
                        <a href='/Dashboard' style='background: #10b981; color: white; padding: 12px 24px;
                                           text-decoration: none; border-radius: 8px; display: inline-block;'>
                            View Dashboard
=======
                        <a href='#' style='background: #10b981; color: white; padding: 12px 24px;
                                           text-decoration: none; border-radius: 8px; display: inline-block;'>
                            View Certificate
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
                        </a>
                    </div>
                    <div style='padding: 20px; text-align: center; color: #6b7280;'>
                        <p>LearnHub — Learn Without Limits</p>
                    </div>
                </div>";

<<<<<<< HEAD
            await SaveToInboxAsync(userId, subject, body);
        }

        public async Task SendInvoiceAsync(string toEmail, string studentName,
            string courseName, decimal price, string instructorName,
            int enrollmentId, string userId)
        {
            var subject = $"Invoice - {courseName} Enrollment";
            var date = DateTime.UtcNow.ToString("MMMM dd, yyyy");
            var invoiceNumber = $"INV-{enrollmentId:D6}";
            var amount = price == 0 ? "Free" : $"${price:0.00}";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; background:#f4f6f9; margin:0; padding:0; }}
        .wrapper {{ max-width:600px; margin:40px auto; background:white; border-radius:16px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.1); }}
        .header {{ background:linear-gradient(135deg, #4f46e5, #7c3aed); padding:40px; text-align:center; }}
        .header h1 {{ color:white; margin:0; font-size:28px; }}
        .header p {{ color:rgba(255,255,255,0.85); margin:8px 0 0; }}
        .body {{ padding:40px; }}
        .invoice-row {{ display:flex; justify-content:space-between; margin-bottom:30px; }}
        .invoice-row div {{ font-size:14px; color:#6b7280; }}
        .invoice-row div strong {{ display:block; color:#111827; font-size:16px; margin-bottom:4px; }}
        .divider {{ border:none; border-top:1px solid #e5e7eb; margin:20px 0; }}
        .course-box {{ background:#f9fafb; border-radius:12px; padding:20px; margin:20px 0; border:1px solid #e5e7eb; }}
        .course-box h3 {{ margin:0 0 8px; color:#111827; }}
        .course-box p {{ margin:4px 0; color:#6b7280; font-size:14px; }}
        .total-box {{ background:linear-gradient(135deg, #4f46e5, #7c3aed); border-radius:12px; padding:20px; text-align:center; margin:20px 0; }}
        .total-box p {{ color:rgba(255,255,255,0.85); margin:0 0 4px; font-size:14px; }}
        .total-box h2 {{ color:white; margin:0; font-size:32px; }}
        .footer {{ background:#f9fafb; padding:30px; text-align:center; border-top:1px solid #e5e7eb; }}
        .footer p {{ color:#9ca3af; font-size:13px; margin:4px 0; }}
        .btn {{ display:inline-block; background:linear-gradient(135deg, #4f46e5, #7c3aed); color:white; padding:12px 32px; border-radius:50px; text-decoration:none; font-weight:600; margin-top:16px; }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='header'>
            <h1>🎓 LearnHub</h1>
            <p>Your enrollment is confirmed!</p>
        </div>
        <div class='body'>
            <p style='color:#374151; font-size:16px;'>Hi <strong>{studentName}</strong>,</p>
            <p style='color:#6b7280;'>Thank you for enrolling! Here is your invoice.</p>
            <div class='invoice-row'>
                <div>
                    <strong>{invoiceNumber}</strong>
                    Invoice Number
                </div>
                <div style='text-align:right;'>
                    <strong>{date}</strong>
                    Date
                </div>
            </div>
            <hr class='divider' />
            <div class='course-box'>
                <h3>📚 {courseName}</h3>
                <p>👨‍🏫 Instructor: <strong>{instructorName}</strong></p>
                <p>📅 Enrolled: <strong>{date}</strong></p>
                <p>♾️ Access: <strong>Lifetime</strong></p>
            </div>
            <div class='total-box'>
                <p>Total Amount Paid</p>
                <h2>{amount}</h2>
            </div>
            <p style='color:#6b7280; font-size:14px; text-align:center;'>
                You now have full access to all lessons in this course.
            </p>
            <div style='text-align:center;'>
                <a href='/Course/MyCourses' class='btn'>Start Learning Now →</a>
            </div>
        </div>
        <div class='footer'>
            <p>© 2026 LearnHub. All rights reserved.</p>
            <p>This is an automated invoice. Please keep it for your records.</p>
        </div>
    </div>
</body>
</html>";

            // Save to inbox only — no SMTP
            await SaveToInboxAsync(userId, subject, body);
=======
            await SendEmailAsync(toEmail, toName, subject, body);
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
        }
    }
}