// SmartQuizAssessmentSystem/Controllers/AccountController.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IEducationMediumService _mediumService;
        private readonly IClassService _classService;

        public AccountController(
            SignInManager<QuizSystemUser> signInManager,
            UserManager<QuizSystemUser> userManager,
            IAccountService accountService,
            IEducationMediumService mediumService,
            IClassService classService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
            _mediumService = mediumService;
            _classService = classService;
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
                return RedirectToAction("Dashboard", "AdminDashboard");

            if (roles.Contains("Instructor"))
                return RedirectToAction("Dashboard", "InstructorDashboard");

            if (roles.Contains("Student"))
                return RedirectToAction("Dashboard", "StudentDashboard");

            return RedirectToAction("Index", "Home");
        }

        // REGISTER SELECTOR

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // STUDENT REGISTER

        [HttpGet]
        public async Task<IActionResult> RegisterStudent()
        {
            var model = new StudentAddViewModel
            {
                Role = "Student"
            };

            await PopulateMediumAndClassDropdownsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(StudentAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMediumAndClassDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            var result = await _accountService.RegisterStudentAsync(model);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                await PopulateMediumAndClassDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        // INSTRUCTOR REGISTER

        [HttpGet]
        public async Task<IActionResult> RegisterInstructor()
        {
            var model = new InstructorAddViewModel
            {
                Role = "Instructor"
            };
            ViewBag.MediumList = await GetMediumSelectListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterInstructor(InstructorAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MediumList = await GetMediumSelectListAsync();
                return View(model);
            }

            var result = await _accountService.RegisterInstructorAsync(model);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                ViewBag.MediumList = await GetMediumSelectListAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        // LOGOUT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Dropdown helpers

        private async Task<IEnumerable<SelectListItem>> GetMediumSelectListAsync()
        {
            var mediums = await _mediumService.GetAllAsync();
            return mediums.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetClassSelectListAsync()
        {
            var classes = await _classService.GetAllAsync();
            return classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name.ToString()

            });
        }

        private async Task PopulateMediumAndClassDropdownsAsync(long? mediumId = null, long? classId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

            var classes = await _classService.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
        }
    }
}
