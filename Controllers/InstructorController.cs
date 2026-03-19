using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;
<<<<<<< HEAD
using SimpleWebsite.Services;
=======
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;
<<<<<<< HEAD
        private readonly NotificationService notificationService;

        public InstructorController(AppDbContext context, UserManager<Users> userManager, NotificationService notificationService)
        {
            this.context = context;
            this.userManager = userManager;
            this.notificationService = notificationService;
=======

        public InstructorController(AppDbContext context, UserManager<Users> userManager)
        {
            this.context = context;
            this.userManager = userManager;
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
        }

        // ── My Courses ────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);
            var courses = await context.Courses
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .Include(c => c.Category)
                .Where(c => c.InstructorId == userId)
                .ToListAsync();

            return View(courses);
        }

        // ── Create Course ─────────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await context.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course model, IFormFile? thumbnailFile)
        {
            if (ModelState.IsValid)
            {
                model.InstructorId = userManager.GetUserId(User);
                model.CreatedAt = DateTime.UtcNow;

                // Handle image upload
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/courses");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(thumbnailFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await thumbnailFile.CopyToAsync(stream);
                    model.Thumbnail = $"/uploads/courses/{fileName}";
                }

                context.Courses.Add(model);
                await context.SaveChangesAsync();
<<<<<<< HEAD

                // ← Notify Admin here
                var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
                foreach (var admin in adminUsers)
                {
                    await notificationService.SendAsync(
                        admin.Id,
                        $"New course created: '{model.Title}' by {User.Identity?.Name}",
                        "/Admin/Courses"
                    );
                }

=======
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = await context.Categories.ToListAsync();
            return View(model);
        }

        // ── Edit Course ───────────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var userId = userManager.GetUserId(User);
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == userId);

            if (course == null) return NotFound();

            ViewBag.Categories = await context.Categories.ToListAsync();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course model, IFormFile? thumbnailFile)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);
                var course = await context.Courses
                    .FirstOrDefaultAsync(c => c.CourseId == model.CourseId && c.InstructorId == userId);

                if (course == null) return NotFound();

                course.Title = model.Title;
                course.Description = model.Description;
                course.Price = model.Price;
                course.IsPublished = model.IsPublished;
                course.CategoryId = model.CategoryId;

                // Handle image upload
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(course.Thumbnail))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.Thumbnail.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/courses");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(thumbnailFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await thumbnailFile.CopyToAsync(stream);
                    course.Thumbnail = $"/uploads/courses/{fileName}";
                }

                await context.SaveChangesAsync();
                TempData["Success"] = "Course updated successfully!";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = await context.Categories.ToListAsync();
            return View(model);
        }

        // ── Delete Course ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = userManager.GetUserId(User);
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == userId);

            if (course == null) return NotFound();

            // Delete image file if exists
            if (!string.IsNullOrEmpty(course.Thumbnail))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.Thumbnail.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            TempData["Success"] = "Course deleted successfully!";
            return RedirectToAction("Index");
        }

        // ── Manage Lessons ────────────────────────────────────────
        public async Task<IActionResult> Lessons(int courseId)
        {
            var userId = userManager.GetUserId(User);
            var course = await context.Courses
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.InstructorId == userId);

            if (course == null) return NotFound();

            return View(course);
        }

        // ── Add Lesson ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLesson(Lesson model, IFormFile? videoFile)
        {
            var userId = userManager.GetUserId(User);
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == model.CourseId && c.InstructorId == userId);

            if (course == null) return NotFound();

            var lastOrder = await context.Lessons
                .Where(l => l.CourseId == model.CourseId)
                .MaxAsync(l => (int?)l.Order) ?? 0;

            model.Order = lastOrder + 1;

            // Handle video upload
            if (videoFile != null && videoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(videoFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await videoFile.CopyToAsync(stream);
                model.VideoUrl = $"/uploads/videos/{fileName}";
            }

            context.Lessons.Add(model);
            await context.SaveChangesAsync();

            TempData["Success"] = "Lesson added successfully!";
            return RedirectToAction("Lessons", new { courseId = model.CourseId });
        }

        public async Task<IActionResult> EditLesson(int lessonId)
        {
            var lesson = await context.Lessons.FindAsync(lessonId);
            if (lesson == null) return NotFound();
            return View(lesson);
        }

        // ── Edit Lesson ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(Lesson model, IFormFile? videoFile)
        {
            var lesson = await context.Lessons.FindAsync(model.LessonId);
            if (lesson == null) return NotFound();

            lesson.Title = model.Title;
            lesson.Duration = model.Duration;
            lesson.IsPreview = model.IsPreview;
            lesson.Order = model.Order;

            if (videoFile != null && videoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/videos");
                Directory.CreateDirectory(uploadsFolder);
                var extension = Path.GetExtension(videoFile.FileName).ToLower();
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await videoFile.CopyToAsync(stream);
                lesson.VideoUrl = $"/uploads/videos/{fileName}";
            }
            else if (!string.IsNullOrEmpty(model.VideoUrl))
            {
                lesson.VideoUrl = model.VideoUrl;
            }

            await context.SaveChangesAsync();
            TempData["Success"] = "Lesson updated!";
            return RedirectToAction("Lessons", new { courseId = lesson.CourseId });
        }

        // ── Delete Lesson ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int lessonId, int courseId)
        {
            var lesson = await context.Lessons.FindAsync(lessonId);
            if (lesson == null) return NotFound();

            context.Lessons.Remove(lesson);
            await context.SaveChangesAsync();

            TempData["Success"] = "Lesson deleted!";
            return RedirectToAction("Lessons", new { courseId });
        }

        // ── Toggle Publish ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var userId = userManager.GetUserId(User);
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == userId);

            if (course == null) return NotFound();

            course.IsPublished = !course.IsPublished;
            await context.SaveChangesAsync();

            TempData["Success"] = course.IsPublished ? "Course published!" : "Course unpublished!";
            return RedirectToAction("Index");
        }

        // ── Earnings ──────────────────────────────────────────────
        public async Task<IActionResult> Earnings()
        {
            var userId = userManager.GetUserId(User);

            var courses = await context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == userId)
                .ToListAsync();

            var earnings = courses.Select(c => new
            {
                CourseTitle = c.Title,
                Students = c.Enrollments.Count,
                Price = c.Price,
                Total = c.Price * c.Enrollments.Count,
                IsPublished = c.IsPublished,
                Lessons = c.Lessons.Count
            }).ToList();

            ViewBag.TotalEarnings = earnings.Sum(e => e.Total);
            ViewBag.TotalStudents = earnings.Sum(e => e.Students);
            ViewBag.TotalCourses = courses.Count;
            ViewBag.Earnings = earnings;

            return View();
        }
    }
}