using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleWebsite.Data;
using SimpleWebsite.Models;

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly AppDbContext context;

        public CouponController(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await context.Coupons
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(coupons);
        }

        // ── Create Coupon (Admin) ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon model)
        {
            if (ModelState.IsValid)
            {
                model.Code = model.Code.ToUpper().Trim();
                model.CreatedAt = DateTime.UtcNow;
                context.Coupons.Add(model);
                await context.SaveChangesAsync();
                TempData["Success"] = "Coupon created!";
            }
            return RedirectToAction("Index");
        }

        // ── Edit Coupon (Admin) ─────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon model)
        {
            if (ModelState.IsValid)
            {
                var coupon = await context.Coupons.FindAsync(model.CouponId);
                if (coupon == null) return NotFound();

                coupon.Code = model.Code.ToUpper().Trim();
                coupon.DiscountPercent = model.DiscountPercent;
                coupon.ExpiryDate = model.ExpiryDate;
                coupon.MaxUses = model.MaxUses;
                coupon.IsActive = model.IsActive;

                await context.SaveChangesAsync();
                TempData["Success"] = "Coupon updated!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ── Toggle Coupon Active/Inactive (Admin) ─────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var coupon = await context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                coupon.IsActive = !coupon.IsActive;
                await context.SaveChangesAsync();
                TempData["Success"] = coupon.IsActive ? "Coupon activated!" : "Coupon deactivated!";
            }
            return RedirectToAction("Index");
        }

        // ── Delete Coupon (Admin) ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                context.Coupons.Remove(coupon);
                await context.SaveChangesAsync();
                TempData["Success"] = "Coupon deleted!";
            }
            return RedirectToAction("Index");
        }

        // ── Validate coupon via AJAX ──────────────────────────────
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Validate(string code, decimal originalPrice)
        {
            var coupon = await context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code.ToUpper().Trim() &&
                                          c.IsActive &&
                                          c.ExpiryDate >= DateTime.UtcNow &&
                                          c.UsedCount < c.MaxUses);

            if (coupon == null)
                return Json(new { success = false, message = "Invalid or expired coupon." });

            var discount = originalPrice * (coupon.DiscountPercent / 100);
            var newPrice = originalPrice - discount;

            return Json(new
            {
                success = true,
                discountPercent = coupon.DiscountPercent,
                discount = discount,
                newPrice = newPrice,
                message = $"Coupon applied! {coupon.DiscountPercent}% off"
            });
        }
    }
}