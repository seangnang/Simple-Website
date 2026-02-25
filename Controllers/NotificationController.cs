using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleWebsite.Models;
using SimpleWebsite.Services;

namespace SimpleWebsite.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NotificationService notificationService;
        private readonly UserManager<Users> userManager;

        public NotificationController(NotificationService notificationService, UserManager<Users> userManager)
        {
            this.notificationService = notificationService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            var notifications = await notificationService.GetUserNotificationsAsync(userId!);
            await notificationService.MarkAllReadAsync(userId!);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = userManager.GetUserId(User);
            await notificationService.MarkAllReadAsync(userId!);
            return Ok();
        }

        public async Task<IActionResult> GetCount()
        {
            var userId = userManager.GetUserId(User);
            var count = await notificationService.GetUnreadCountAsync(userId!);
            return Json(new { count });
        }
    }
}