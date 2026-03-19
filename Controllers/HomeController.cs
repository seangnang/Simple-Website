using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;

namespace SimpleWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext context;

        public HomeController(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Featured courses
            ViewBag.Courses = await context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Include(c => c.Category)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .Take(6)
                .ToListAsync();

            // Stats
            ViewBag.TotalCourses = await context.Courses.CountAsync(c => c.IsPublished);
            ViewBag.TotalStudents = await context.Users.CountAsync();
            ViewBag.TotalInstructors = (await context.UserRoles
                .Join(context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur, r })
                .Where(x => x.r.Name == "Instructor")
                .ToListAsync()).Count;

            // Categories
            ViewBag.Categories = await context.Categories
                .Include(c => c.Courses)
                .ToListAsync();

            return View();
        }
<<<<<<< HEAD

        public IActionResult Calender() => View();
=======
>>>>>>> 12cce462aa52a2c7f4e0f4941ece9d4f5ec0d21f
    }
}