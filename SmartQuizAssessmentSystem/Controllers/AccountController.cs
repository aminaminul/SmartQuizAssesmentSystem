using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemRepository.Data;

namespace SmartQuizAssessmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<QuizSystemUser> signInManager;
        private readonly UserManager<QuizSystemUser> userManager;
        private readonly RoleManager<QuizSystemRole> roleManager;
        private readonly AppDbContext context;

        public AccountController(SignInManager<QuizSystemUser> signInManager,UserManager<QuizSystemUser> userManager,RoleManager<QuizSystemRole> roleManager,AppDbContext context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        //LOGIN

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

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if (await userManager.IsInRoleAsync(user, "Instructor"))
            {
                return RedirectToAction("Dashboard", "Instructor");
            }
            else
            {
                return RedirectToAction("Dashboard", "Student");
            }
        }

        //REGISTER

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.RegistrationType))
            {
                if (model.RegistrationType == "Student")
                    model.Role = "Student";
                else if (model.RegistrationType == "Instructor")
                    model.Role = "Instructor";
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.Role != "Student" && model.Role != "Instructor")
            {
                ModelState.AddModelError("Role", "Invalid role selection.");
                return View(model);
            }

            var user = new QuizSystemUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await userManager.AddToRoleAsync(user, model.Role);

            //Student
            if (model.Role == "Student")
            {
                var student = new Student
                {
                    Name = $"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                };
                context.Student.Add(student);
            }
            //Instructor
            else if (model.Role == "Instructor")
            {
                var instructor = new Instructor
                {
                    Name = $"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    HscPassingInstrutute = model.HscPassingInstitute,
                    HscGrade = model.HscGrade,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    EducationMedium = model.EducationMediumId
                };
                context.Instructor.Add(instructor);
            }

            await context.SaveChangesAsync();

            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Login", "Account");
        }

        //VERIFY EMAIL

        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }

        //CHANGE PASSWORD
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            var result = await userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        //LOGOUT 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
