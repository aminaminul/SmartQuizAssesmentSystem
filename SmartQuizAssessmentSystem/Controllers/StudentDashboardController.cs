using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IStudentDashboardService _dashboardService;
        private readonly IStudentQuizService _studentQuizService;
        private readonly IProfileUpdateService _profileService;
        private readonly IEducationMediumService _mediumService;
        private readonly IClassService _classService;
        public StudentDashboardController(
            UserManager<QuizSystemUser> userManager,
            IStudentDashboardService dashboardService,
            IStudentQuizService studentQuizService,
             IProfileUpdateService profileService,
            IEducationMediumService mediumService,
            IClassService classService)
        {
            _userManager = userManager;
            _dashboardService = dashboardService;
            _studentQuizService = studentQuizService;
            _profileService = profileService;
            _mediumService = mediumService;
            _classService = classService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _dashboardService.GetDashboardAsync(user.Id);
            model.StudentName = $"{user.FirstName} {user.LastName}";
            
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(model);
        }
        public async Task<IActionResult> AvailableQuizzes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var model = await _dashboardService.GetDashboardAsync(user.Id);
            return View(model.AvailableQuizzes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuiz(long quizId)
        {
            var user = await _userManager.GetUserAsync(User);
            var attempt = await _studentQuizService.StartAttemptAsync(quizId, user.Id);
            return RedirectToAction("Attempt", "StudentQuiz", new { id = attempt.Id });
        }
        public async Task<IActionResult> RecentAttempts()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _dashboardService.GetDashboardAsync(user.Id);
            return View(model.RecentAttempts);
        }
        public async Task<IActionResult> AllAttempts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var model = await _dashboardService.GetDashboardAsync(user.Id);
            return View(model.RecentAttempts);
        }
        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetStudentProfileForEditAsync(user.Id);
            await PopulateStudentDropdownsAsync(vm.EducationMediumId, vm.ClassId);

            return View("ViewProfile", vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetStudentProfileForEditAsync(user.Id);
            await PopulateStudentDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View("UpdateProfile", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(StudentProfileUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateStudentDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }

            try
            {
                await _profileService.RequestStudentProfileUpdateAsync(model);
                TempData["SuccessMessage"] = "Profile update request submitted for admin approval.";
                return RedirectToAction(nameof(ViewProfile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateStudentDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }
        }

        private async Task PopulateStudentDropdownsAsync(long? selectedMediumId = null, long? selectedClassId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            var classes = await _classService.GetAllAsync(null);

            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedClassId);
        }
    }

}
