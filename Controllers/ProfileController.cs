using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleWebsite.Models;
using SimpleWebsite.ViewModels;

namespace SimpleWebsite.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;

        public ProfileController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Fullname = user.Fullname,
                Email = user.Email ?? string.Empty,
            };

            ViewBag.ProfilePicture = user.ProfilePicture;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                user.Fullname = model.Fullname;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    TempData["Success"] = "Profile updated successfully!";
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // ── Upload Profile Picture ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPicture(IFormFile pictureFile)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (pictureFile != null && pictureFile.Length > 0)
            {
                var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(pictureFile.FileName).ToLower();
                if (!allowedTypes.Contains(extension))
                {
                    TempData["Error"] = "Only JPG, PNG or WEBP allowed.";
                    return RedirectToAction("Index");
                }

                if (pictureFile.Length > 2 * 1024 * 1024)
                {
                    TempData["Error"] = "Image must be under 2MB.";
                    return RedirectToAction("Index");
                }

                // Delete old picture
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/profiles");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await pictureFile.CopyToAsync(stream);

                user.ProfilePicture = $"/uploads/profiles/{fileName}";
                await userManager.UpdateAsync(user);
                TempData["Success"] = "Profile picture updated!";
            }

            return RedirectToAction("Index");
        }

        // ── Change Password ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("Index");
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password changed successfully!";
            }
            else
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Index");
        }
    }
}