using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleWebsite.Models;
using SimpleWebsite.Services;
using SimpleWebsite.ViewModels;

namespace SimpleWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly NotificationService notificationService;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager , NotificationService notificationService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.notificationService = notificationService;
        }

        public IActionResult Index() => View();

        // ── Login ────────────────────────────────────────────────
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (await userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "Admin");

                    if (await userManager.IsInRoleAsync(user, "Instructor"))
                        return RedirectToAction("Index", "Instructor");

                    // Default → Student
                    return RedirectToAction("Index", "Course");
                }
                ModelState.AddModelError("", "Email or password is incorrect.");
            }
            return View(model);
        }

        // GET
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    Fullname = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Student");

                    // ← Notify Admin here inside Succeeded block
                    var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
                    foreach (var admin in adminUsers)
                    {
                        await notificationService.SendAsync(
                            admin.Id,
                            $"New user registered: {model.Name} ({model.Email})",
                            "/Admin/Index"
                        );
                    }

                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // ── Logout ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Home");
        }
    }
}