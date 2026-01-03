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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuiz(long quizId)
        {
            var user = await _userManager.GetUserAsync(User);
            var attempt = await _studentQuizService.StartAttemptAsync(quizId, user.Id);
            return RedirectToAction("Attempt", "StudentQuiz", new { id = attempt.Id });
        }
    }
}
