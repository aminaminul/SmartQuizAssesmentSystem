using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize]
    public class PerformanceController : Controller
    {
        private readonly IPerformanceService _performanceService;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IEducationMediumService _mediumService;
        private readonly IClassService _classService;

        public PerformanceController(
            IPerformanceService performanceService,
            UserManager<QuizSystemUser> userManager,
            IEducationMediumService mediumService,
            IClassService classService)
        {
            _performanceService = performanceService;
            _userManager = userManager;
            _mediumService = mediumService;
            _classService = classService;
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Student(long? mediumId, long? classId)
        {
            ViewData["Title"] = "Student Performance";
            var viewModel = await _performanceService.GetStudentPerformanceAsync(null, classId, mediumId);
            await PopulateDropdowns(mediumId, classId);
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Class(long? mediumId)
        {
            ViewData["Title"] = "Class Performance";
            var viewModel = await _performanceService.GetClassPerformanceAsync(null, mediumId);
            await PopulateDropdowns(mediumId, null);
            return View(viewModel);
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> EducationMedium()
        {
            ViewData["Title"] = "Education Medium Performance";
            var viewModel = await _performanceService.GetMediumPerformanceAsync();
            return View(viewModel);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyPerformance()
        {
            ViewData["Title"] = "My Performance";
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var viewModel = await _performanceService.GetMyPerformanceAsync(user.Id);
            return View(viewModel);
        }

        private async Task PopulateDropdowns(long? mediumId, long? classId)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

            var classes = await _classService.GetAllAsync(mediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
        }

        [HttpGet]
        public async Task<IActionResult> GetClassesByMedium(long? mediumId)
        {
            var classes = await _classService.GetAllAsync(mediumId);
            return Json(classes.Select(c => new { id = c.Id, name = c.Name }));
        }
    }
}
