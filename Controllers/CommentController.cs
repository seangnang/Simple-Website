using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;

        public CommentController(AppDbContext context, UserManager<Users> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int lessonId, int courseId, string content)
        {
            var userId = userManager.GetUserId(User);

            var comment = new Comment
            {
                LessonId = lessonId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            TempData["Success"] = "Comment added!";
            return RedirectToAction("Learn", "Course", new { id = courseId, lessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId, int courseId, int lessonId)
        {
            var userId = userManager.GetUserId(User);
            var comment = await context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);

            if (comment != null)
            {
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
                TempData["Success"] = "Comment deleted.";
            }

            return RedirectToAction("Learn", "Course", new { id = courseId, lessonId });
        }
    }
}