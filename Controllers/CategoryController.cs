using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext context;

        public CategoryController(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await context.Categories
                .Include(c => c.Courses)
                .ToListAsync();
            return View(categories);
        }

        //-- Create Category (Admin) ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categories model)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                TempData["Success"] = "Category created!";
            }
            return RedirectToAction("Index");
        }

        //-- Edit Category (Admin) ─────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Categories model)
        {
            if (ModelState.IsValid)
            {
                var category = await context.Categories.FindAsync(model.CategoryId);
                if (category == null) return NotFound();

                category.Name = model.Name;
                category.Description = model.Description;
                category.Icon = model.Icon;

                await context.SaveChangesAsync();
                TempData["Success"] = "Category updated!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //-- Delete Category (Admin) ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                TempData["Success"] = "Category deleted!";
            }
            return RedirectToAction("Index");
        }
    }
}