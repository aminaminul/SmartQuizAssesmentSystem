using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;
using SmartQuizAssessmentSystem.Services;

namespace SmartQuizAssessmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<QuizSystemUser> _signInManager;
        private readonly IAccountService _accountService;

        public AccountController(
            SignInManager<QuizSystemUser> signInManager,
            IAccountService accountService)
        {
            _signInManager = signInManager;
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

            return RedirectToAction("Index", "Home");
        }

        // REGISTER

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //Student/Instructor
            if (string.IsNullOrEmpty(model.FirstName) &&
                !string.IsNullOrEmpty(model.RegistrationType))
            {
                model.Role = model.RegistrationType;
                return View(model);
            }

            //Full form submission
            if (!ModelState.IsValid)
                return View(model);

            if (model.Role != "Student" && model.Role != "Instructor")
            {
                ModelState.AddModelError("Role", "Invalid role selection.");
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
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
