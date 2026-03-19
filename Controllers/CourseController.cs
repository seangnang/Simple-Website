using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;
using SimpleWebsite.Services;

namespace SimpleWebsite.Controllers
{
    public class CourseController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;
        private readonly EmailService emailService;
<<<<<<< HEAD
        private readonly NotificationService notificationService;

        public CourseController(AppDbContext context, UserManager<Users> userManager, EmailService emailService, NotificationService notificationService)
=======

        public CourseController(AppDbContext context, UserManager<Users> userManager, EmailService emailService)
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
        {
            this.context = context;
            this.userManager = userManager;
            this.emailService = emailService;
<<<<<<< HEAD
            this.notificationService = notificationService;
=======
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
        }

        // ── Browse All Courses (Public) ───────────────────────────
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var categories = await context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;

            var courses = context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Category)
                .Where(c => c.IsPublished);

            if (!string.IsNullOrEmpty(search))
                courses = courses.Where(c => c.Title.Contains(search) ||
                                             c.Description.Contains(search));

            if (categoryId.HasValue)
                courses = courses.Where(c => c.CategoryId == categoryId);

            return View(await courses.ToListAsync());
        }

        // ── Course Detail ─────────────────────────────────────────
        public async Task<IActionResult> Details(int id)
        {
            var course = await context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            var userId = userManager.GetUserId(User);
            var isEnrolled = await context.Enrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == id);
            var hasReviewed = await context.Reviews
                .AnyAsync(r => r.StudentId == userId && r.CourseId == id);
            var reviews = await context.Reviews
                .Include(r => r.Student)
                .Where(r => r.CourseId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.HasReviewed = hasReviewed;
            ViewBag.Reviews = reviews;
            ViewBag.AvgRating = Math.Round(avgRating, 1);
            ViewBag.CurrentUserId = userId;

            return View(course);
        }

        //Preview course content (for enrolled students)
        public async Task<IActionResult> Preview(int lessonId)
        {
            var lesson = await context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId && l.IsPreview);

            if (lesson == null) return NotFound();

            return View(lesson);
        }

        // ── Enroll in Course ──────────────────────────────────────
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userId = userManager.GetUserId(User);
<<<<<<< HEAD
            var alreadyEnrolled = await context.Enrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);
=======

            var alreadyEnrolled = await context.Enrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);

>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
            if (alreadyEnrolled)
            {
                TempData["Error"] = "You are already enrolled in this course.";
                return RedirectToAction("Details", new { id = courseId });
            }
<<<<<<< HEAD
=======

>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
            var enrollment = new Enrollment
            {
                StudentId = userId!,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow
            };
<<<<<<< HEAD
            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

            // Load course with instructor and student
            var course = await context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            var student = await userManager.FindByIdAsync(userId!);

            // Send invoice email
            try
            {
                if (student?.Email != null && course != null)
                {
                    await emailService.SendInvoiceAsync(
                        student.Email,
                        student.Fullname,
                        course.Title,
                        course.Price,
                        course.Instructor?.Fullname ?? "Instructor",
                        enrollment.EnrollmentId,
                        student.Id
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email error: {ex.Message}");
            }

            // Notify student
            await notificationService.SendAsync(
                userId!,
                $"You have successfully enrolled in {course?.Title}!",
                $"/Course/Learn/{courseId}"
            );

            // Notify instructor
            if (course?.InstructorId != null)
            {
                await notificationService.SendAsync(
                    course.InstructorId,
                    $"{student?.Fullname} enrolled in your course '{course.Title}'",
                    "/Instructor/Index"
                );
            }

            // Notify Admin
            var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in adminUsers)
            {
                await notificationService.SendAsync(
                    admin.Id,
                    $"{student?.Fullname} enrolled in '{course?.Title}'",
                    "/Admin/Courses"
                );
            }

=======

            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
            TempData["Success"] = "Successfully enrolled!";
            return RedirectToAction("Learn", new { id = courseId });
        }

        // ── Student Learning Page ─────────────────────────────────
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Learn(int id, int? lessonId)
        {
            var userId = userManager.GetUserId(User);

            var enrollment = await context.Enrollments
                .Include(e => e.LessonProgresses)
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == id);

            if (enrollment == null)
                return RedirectToAction("Details", new { id });

            var course = await context.Courses
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null) return NotFound();

            var currentLesson = lessonId.HasValue
                ? course.Lessons.FirstOrDefault(l => l.LessonId == lessonId)
                : course.Lessons.FirstOrDefault();

            // Load comments for current lesson
            var comments = new List<Comment>();
            if (currentLesson != null)
            {
                comments = await context.Comments
                    .Include(c => c.User)
                    .Where(c => c.LessonId == currentLesson.LessonId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }

            ViewBag.Enrollment = enrollment;
            ViewBag.CurrentLesson = currentLesson;
            ViewBag.Comments = comments;
            ViewBag.CurrentUserId = userId;

            return View(course);
        }

        // ── Mark Lesson Complete ──────────────────────────────────
        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int lessonId, int courseId)
        {
            var userId = userManager.GetUserId(User);

            var enrollment = await context.Enrollments
                .Include(e => e.LessonProgresses)
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == courseId);

            if (enrollment == null) return NotFound();

            // Check if already marked
            var alreadyDone = enrollment.LessonProgresses
                .Any(lp => lp.LessonId == lessonId);

            if (!alreadyDone)
            {
                var progress = new LessonProgress
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    WatchedAt = DateTime.UtcNow
                };

                context.LessonProgresses.Add(progress);

                // Check if all lessons completed
                var totalLessons = await context.Lessons
                    .CountAsync(l => l.CourseId == courseId);

                if (enrollment.LessonProgresses.Count + 1 >= totalLessons)
                {
                    enrollment.IsCompleted = true;
                    TempData["Success"] = "Congratulations! You completed the course!";
                }

                await context.SaveChangesAsync();
            }

            return RedirectToAction("Learn", new { id = courseId, lessonId });
        }

        // ── My Courses (Student) ──────────────────────────────────
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = userManager.GetUserId(User);

            var enrollments = await context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Lessons)
                .Include(e => e.LessonProgresses)
                .Where(e => e.StudentId == userId)
                .ToListAsync();

            return View(enrollments);
        }
    }
}