using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IStudentDashboardService _dashboardService;
        private readonly IStudentQuizService _studentQuizService;

        public StudentDashboardController(
            UserManager<QuizSystemUser> userManager,
            IStudentDashboardService dashboardService,
            IStudentQuizService studentQuizService)
        {
            _userManager = userManager;
            _dashboardService = dashboardService;
            _studentQuizService = studentQuizService;
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
    }

}
