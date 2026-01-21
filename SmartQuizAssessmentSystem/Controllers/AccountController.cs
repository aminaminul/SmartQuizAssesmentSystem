
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

        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        

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

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        

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

        [HttpGet]
        public async Task<JsonResult> GetClassesByMedium(long? mediumId)
        {
            
            
            
            if (mediumId == 0) mediumId = null;

            var classes = await _classService.GetAllAsync(mediumId);
            return Json(classes.Select(c => new { id = c.Id, name = c.Name }));
        }

        private async Task PopulateMediumAndClassDropdownsAsync(long? mediumId = null, long? classId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

            var classes = await _classService.GetAllAsync(mediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
        }

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    return RedirectToAction("ResetPassword", "Account", new { token = token, email = model.Email });
                }

                ModelState.AddModelError(string.Empty, "Email not found.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
