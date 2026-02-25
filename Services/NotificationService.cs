using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Services
{
    public class NotificationService
    {
        private readonly AppDbContext context;

        public NotificationService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task SendAsync(string userId, string message, string link = "")
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Link = link,
                CreatedAt = DateTime.UtcNow
            };
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await context.SaveChangesAsync();
        }
    }
}