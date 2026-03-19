using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;

        public DashboardController(AppDbContext context, UserManager<Users> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            if (User.IsInRole("Admin"))
            {
                // Admin stats
                ViewBag.TotalUsers = await context.Users.CountAsync();
                ViewBag.TotalCourses = await context.Courses.CountAsync();
                ViewBag.TotalEnrollments = await context.Enrollments.CountAsync();
                ViewBag.TotalCategories = await context.Categories.CountAsync();
<<<<<<< HEAD
                var instructors = await userManager.GetUsersInRoleAsync("Instructor");
                ViewBag.TotalInstructors = instructors.Count;
=======
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
                ViewBag.RecentUsers = await context.Users
                    .OrderByDescending(u => u.Id)
                    .Take(5)
                    .ToListAsync();
                ViewBag.RecentCourses = await context.Courses
                    .Include(c => c.Instructor)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            else if (User.IsInRole("Instructor"))
            {
                // Instructor stats
                var courses = await context.Courses
                    .Include(c => c.Lessons)
                    .Include(c => c.Enrollments)
                    .Where(c => c.InstructorId == userId)
                    .ToListAsync();

                ViewBag.TotalCourses = courses.Count;
                ViewBag.TotalStudents = courses.Sum(c => c.Enrollments.Count);
                ViewBag.TotalLessons = courses.Sum(c => c.Lessons.Count);
                ViewBag.PublishedCourses = courses.Count(c => c.IsPublished);
                ViewBag.Courses = courses;
            }
            else
            {
                // Student stats
                var enrollments = await context.Enrollments
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Lessons)
                    .Include(e => e.LessonProgresses)
                    .Where(e => e.StudentId == userId)
                    .ToListAsync();

                ViewBag.TotalEnrolled = enrollments.Count;
                ViewBag.TotalCompleted = enrollments.Count(e => e.IsCompleted);
                ViewBag.InProgress = enrollments.Count(e => !e.IsCompleted);
                ViewBag.TotalLessonsCompleted = enrollments.Sum(e => e.LessonProgresses.Count);
                ViewBag.Enrollments = enrollments;
            }

            return View();
        }
    }
}