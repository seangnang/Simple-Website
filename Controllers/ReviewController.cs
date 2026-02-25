using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;
using SimpleWebsite.Services;

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Student")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;
        private readonly NotificationService notificationService;

        public ReviewController(AppDbContext context, UserManager<Users> userManager, NotificationService notificationService)
        {
            this.context = context;
            this.userManager = userManager;
            this.notificationService = notificationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, int rating, string comment)
        {
            var userId = userManager.GetUserId(User);

            var enrolled = await context.Enrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);

            if (!enrolled)
            {
                TempData["Error"] = "You must be enrolled to review this course.";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            var alreadyReviewed = await context.Reviews
                .AnyAsync(r => r.StudentId == userId && r.CourseId == courseId);

            if (alreadyReviewed)
            {
                TempData["Error"] = "You have already reviewed this course.";
                return RedirectToAction("Details", "Course", new { id = courseId });
            }

            var review = new Review
            {
                CourseId = courseId,
                StudentId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            // Notify Instructor
            var course = await context.Courses.FindAsync(courseId);
            var student = await userManager.FindByIdAsync(userId!);
            if (course?.InstructorId != null)
            {
                await notificationService.SendAsync(
                    course.InstructorId,
                    $"{student?.Fullname} rated your course '{course.Title}' {rating} ⭐",
                    $"/Course/Details/{courseId}"
                );
            }

            TempData["Success"] = "Review submitted successfully!";
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int reviewId, int courseId)
        {
            var userId = userManager.GetUserId(User);
            var review = await context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId && r.StudentId == userId);

            if (review != null)
            {
                context.Reviews.Remove(review);
                await context.SaveChangesAsync();
                TempData["Success"] = "Review deleted.";
            }

            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}