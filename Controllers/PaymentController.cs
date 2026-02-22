using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleWebsite.Data;
using SimpleWebsite.Models;
using Stripe;
using Stripe.Checkout;

namespace SimpleWebsite.Controllers
{
    [Authorize(Roles = "Student")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<Users> userManager;
        private readonly StripeSettings stripeSettings;

        public PaymentController(AppDbContext context, UserManager<Users> userManager, IOptions<StripeSettings> stripeSettings)
        {
            this.context = context;
            this.userManager = userManager;
            this.stripeSettings = stripeSettings.Value;
        }

        // ── Checkout ──────────────────────────────────────────────
        public async Task<IActionResult> Checkout(int courseId)
        {
            var course = await context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return NotFound();

            // If free, enroll directly
            if (course.Price == 0)
                return RedirectToAction("Enroll", "Course", new { courseId });

            ViewBag.Course = course;
            ViewBag.PublishableKey = stripeSettings.PublishableKey;
            return View();
        }

        // ── Create Stripe Session ─────────────────────────────────
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateCheckoutSession(int courseId)
        //{
        //    var course = await context.Courses.FindAsync(courseId);
        //    if (course == null) return NotFound();

        //    var userId = userManager.GetUserId(User);
        //    var user = await userManager.FindByIdAsync(userId!);

        //    var domain = $"{Request.Scheme}://{Request.Host}";

        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>
        //        {
        //            new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = course.Title,
        //                        Description = course.Description
        //                    },
        //                    UnitAmount = (long)(course.Price * 100)
        //                },
        //                Quantity = 1
        //            }
        //        },
        //        Mode = "payment",
        //        SuccessUrl = $"{domain}/Payment/Success?courseId={courseId}&session_id={{CHECKOUT_SESSION_ID}}",
        //        CancelUrl = $"{domain}/Course/Details/{courseId}",
        //        CustomerEmail = user?.Email
        //    };

        //    var service = new SessionService();
        //    var session = await service.CreateAsync(options);

        //    return Redirect(session.Url);
        //}

        // ── Payment Success ───────────────────────────────────────
        public async Task<IActionResult> Success(int courseId, string session_id)
        {
            // Verify payment
            var service = new SessionService();
            var session = await service.GetAsync(session_id);

            if (session.PaymentStatus == "paid")
            {
                var userId = userManager.GetUserId(User);

                // Check if already enrolled
                var alreadyEnrolled = await context.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);

                if (!alreadyEnrolled)
                {
                    var enrollment = new Enrollment
                    {
                        StudentId = userId!,
                        CourseId = courseId,
                        EnrolledAt = DateTime.UtcNow
                    };
                    context.Enrollments.Add(enrollment);
                    await context.SaveChangesAsync();
                }

                TempData["Success"] = "Payment successful! You are now enrolled!";
                return RedirectToAction("Learn", "Course", new { id = courseId });
            }

            TempData["Error"] = "Payment failed. Please try again.";
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        // ── Payment Cancel ────────────────────────────────────────
        public IActionResult Cancel(int courseId)
        {
            TempData["Error"] = "Payment cancelled.";
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        // Coupon or discount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession(int courseId, string? couponCode)
        {
            var course = await context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId!);
            var domain = $"{Request.Scheme}://{Request.Host}";

            // Apply coupon if provided
            var finalPrice = course.Price;
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await context.Coupons
                    .FirstOrDefaultAsync(c => c.Code == couponCode.ToUpper().Trim() &&
                                              c.IsActive &&
                                              c.ExpiryDate >= DateTime.UtcNow &&
                                              c.UsedCount < c.MaxUses);
                if (coupon != null)
                {
                    finalPrice = course.Price - (course.Price * coupon.DiscountPercent / 100);
                    coupon.UsedCount++;
                    await context.SaveChangesAsync();
                }
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = course.Title,
                        Description = course.Description
                    },
                    UnitAmount = (long)(finalPrice * 100)
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/Payment/Success?courseId={courseId}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/Course/Details/{courseId}",
                CustomerEmail = user?.Email
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return Redirect(session.Url);
        }
    }
}