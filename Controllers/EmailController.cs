using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;

        public EmailController(AppDbContext context, UserManager<Users> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            var emails = await context.EmailLogs
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.SentAt)
                .ToListAsync();

            return View(emails);
        }

        public async Task<IActionResult> Read(int id)
        {
            var userId = userManager.GetUserId(User);
            var email = await context.EmailLogs
                .FirstOrDefaultAsync(e => e.EmailLogId == id && e.UserId == userId);

            if (email == null) return NotFound();

            email.IsRead = true;
            await context.SaveChangesAsync();

            return View(email);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userManager.GetUserId(User);
            var email = await context.EmailLogs
                .FirstOrDefaultAsync(e => e.EmailLogId == id && e.UserId == userId);

            if (email != null)
            {
                context.EmailLogs.Remove(email);
                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}