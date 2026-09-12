
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemModel.BusinessRules;
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
        public IActionResult Login(string? role = null)
        {
            return View(new LoginViewModel { Email = "", Password = "", Role = role });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "User account not found.");
                TempData["ErrorMessage"] = "User account not found.";
                return View(model);
            }

            var isApproved = await _accountService.IsUserApprovedAsync(user.Id);
            if (!isApproved)
            {
                await _signInManager.SignOutAsync();
                ModelState.AddModelError(string.Empty, "Your account is pending approval by an administrator or has been deactivated.");
                TempData["ErrorMessage"] = "Your account is pending approval by an administrator.";
                return View(model);
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Role portal check — if user came from a specific portal card, enforce role match
            if (!string.IsNullOrEmpty(model.Role))
            {
                bool roleMatch = model.Role switch
                {
                    "Admin"      => roles.Contains("Admin"),
                    "Instructor" => roles.Contains("Instructor"),
                    "Student"    => roles.Contains("Student"),
                    _            => true
                };

                if (!roleMatch)
                {
                    await _signInManager.SignOutAsync();
                    var portalName = model.Role switch
                    {
                        "Admin"      => "Admin Portal",
                        "Instructor" => "Instructor Portal",
                        "Student"    => "Student Portal",
                        _            => model.Role + " Portal"
                    };
                    ModelState.AddModelError(string.Empty,
                        $"Access denied. This account does not have {model.Role} privileges. Please use the correct portal.");
                    TempData["ErrorMessage"] = $"Access denied. This account does not have {model.Role} privileges.";
                    return View(model);
                }
            }

            if (roles.Contains("Admin"))
            {
                TempData["SuccessMessage"] = "Login successful! Welcome to the Admin Dashboard.";
                return RedirectToAction("Dashboard", "AdminDashboard");
            }

            if (roles.Contains("Instructor"))
            {
                TempData["SuccessMessage"] = $"Login successful! Welcome back, {user.FirstName}.";
                return RedirectToAction("Dashboard", "InstructorDashboard");
            }

            if (roles.Contains("Student"))
            {
                TempData["SuccessMessage"] = $"Login successful! Welcome back, {user.FirstName}.";
                return RedirectToAction("Dashboard", "StudentDashboard");
            }

            TempData["SuccessMessage"] = $"Login successful! Welcome back, {user.FirstName}.";
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
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
                await PopulateMediumAndClassDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            var result = await _accountService.RegisterStudentAsync(model);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                TempData["ErrorMessage"] = "Student registration failed. Please review the errors below.";
                await PopulateMediumAndClassDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            TempData["SuccessMessage"] = "Student registration submitted successfully! Your account is pending administrator approval. You can log in once approved.";
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
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
                ViewBag.MediumList = await GetMediumSelectListAsync();
                return View(model);
            }

            var result = await _accountService.RegisterInstructorAsync(model);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                TempData["ErrorMessage"] = "Instructor registration failed. Please review the errors below.";
                ViewBag.MediumList = await GetMediumSelectListAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = "Instructor registration submitted successfully! Your account is pending administrator approval. You can log in once approved.";
            return RedirectToAction(nameof(Login));
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        

        private async Task<IEnumerable<SelectListItem>> GetMediumSelectListAsync()
        {
            var mediums = await _mediumService.GetAllAsync();
            return mediums.Where(m => m.IsApproved && m.Status == ModelStatus.Active)
                .Select(m => new SelectListItem
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
            var classes = await _classService.GetAllAsync(mediumId);
            return Json(classes.Select(c => new { id = c.Id, name = c.Name }).ToList());
        }

        private async Task PopulateMediumAndClassDropdownsAsync(long? mediumId = null, long? classId = null)
        {
            // Show all non-deleted mediums
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

            // Fetch classes only if a valid medium is selected
            if (mediumId.HasValue && mediumId.Value > 0)
            {
                var classes = await _classService.GetAllAsync(mediumId);
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
            }
            else
            {
                // Ensure the list is empty if no medium is selected
                ViewBag.ClassId = new SelectList(new List<Class>(), "Id", "Name");
            }
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
