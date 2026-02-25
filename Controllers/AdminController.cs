using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;

        public AdminController(AppDbContext context, UserManager<Users> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // ── User Management ───────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var users = await context.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in users)
                userRoles[user.Id] = await userManager.GetRolesAsync(user);

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, roleName);

            TempData["Success"] = $"Role '{roleName}' assigned successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await userManager.RemoveFromRoleAsync(user, roleName);
            TempData["Success"] = "Role removed successfully!";
            return RedirectToAction("Index");
        }

        // ── Course Management ─────────────────────────────────────
        public async Task<IActionResult> Courses()
        {
            var courses = await context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            course.IsPublished = !course.IsPublished;
            await context.SaveChangesAsync();

            TempData["Success"] = course.IsPublished ? "Course published!" : "Course unpublished!";
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            // Delete thumbnail
            if (!string.IsNullOrEmpty(course.Thumbnail))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", course.Thumbnail.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            context.Courses.Remove(course);
            await context.SaveChangesAsync();

            TempData["Success"] = "Course deleted!";
            return RedirectToAction("Courses");
        }
    }
}