using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<QuizSystemUser> _signInManager;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IAccountService _accountService;

        public AccountController(SignInManager<QuizSystemUser> signInManager,UserManager<QuizSystemUser> userManager,IAccountService accountService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
        }

        // LOGIN

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Dashboard", "Admin");

            if (roles.Contains("Instructor"))
                return RedirectToAction("Dashboard", "InstructorDashboard");

            if (roles.Contains("Student"))
                return RedirectToAction("Dashboard", "Student");

            return RedirectToAction("Index", "Home");
        }

        // REGISTER

        // Student / Instructor
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterSelect(string registrationType)
        {
            var model = new RegisterViewModel
            {
                RegistrationType = registrationType,
                Role = registrationType
            };
            ModelState.Clear();
            return View("Register", model);
        }

        //Full Form Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSubmit(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            if (model.Role != "Student" && model.Role != "Instructor")
            {
                ModelState.AddModelError("Role", "Invalid Role Selection.");
                return View("Register", model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Register", model);
            }

            return RedirectToAction("Login", "Account");
        }

        // LOGOUT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
