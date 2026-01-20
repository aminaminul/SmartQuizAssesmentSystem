using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IInstructorService _instructorService;
        private readonly IClassService _classService;
        private readonly IEducationMediumService _mediumService;
        private readonly ISubjectService _subjectService;
        private readonly IProfileUpdateService _profileService;
        private readonly IInstructorDashboardService _dashboardService;
        private readonly UserManager<QuizSystemUser> _userManager;
        public InstructorDashboardController(
            IStudentService studentService,
            IInstructorService instructorService,
            IClassService classService,
            IEducationMediumService mediumService,
            ISubjectService subjectService,
            IProfileUpdateService profileService,
            IInstructorDashboardService dashboardService,
            UserManager<QuizSystemUser> userManager)
        {
            _studentService = studentService;
            _instructorService = instructorService;
            _classService = classService;
            _mediumService = mediumService;
            _subjectService = subjectService;
            _dashboardService = dashboardService;
            _userManager = userManager;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var model = await _dashboardService.GetDashboardAsync(user.Id);
            return View(model);
        }

        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        public async Task<IActionResult> StudentDetails(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Instructors()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        public async Task<IActionResult> InstructorDetails(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        public async Task<IActionResult> Classes()
        {
            var classes = await _classService.GetAllAsync(null);
            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null)
                return NotFound();

            return View(cls);
        }

        public async Task<IActionResult> EducationMediums()
        {
            var mediums = await _mediumService.GetAllAsync();
            return View(mediums);
        }

        public async Task<IActionResult> EducationMediumDetails(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        public async Task<IActionResult> Subjects()
        {
            var subjects = await _subjectService.GetAllAsync();
            return View(subjects);
        }

        public async Task<IActionResult> SubjectDetails(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            await PopulateInstructorDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View("ViewProfile", vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            await PopulateInstructorDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View("UpdateProfile", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(InstructorProfileUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateInstructorDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }

            try
            {
                await _profileService.RequestInstructorProfileUpdateAsync(model);
                TempData["SuccessMessage"] = "Profile update request submitted for admin approval.";
                return RedirectToAction(nameof(ViewProfile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateInstructorDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var model = new ChangePasswordViewModel
            {
                Email = user.Email
            };

            return View("ChangePassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ChangePassword", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Email does not match your account.");
                return View("ChangePassword", model);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View("ChangePassword", model);
            }

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(ViewProfile));
        }
        private async Task PopulateInstructorDropdownsAsync(long? selectedMediumId = null, long? selectedClassId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);

            var classes = await _classService.GetAllAsync(selectedMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedClassId);
        }
    }
}
