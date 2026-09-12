using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentQuizController : Controller
    {
        private readonly IStudentQuizService _studentQuizService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public StudentQuizController(
            IStudentQuizService studentQuizService,
            UserManager<QuizSystemUser> userManager)
        {
            _studentQuizService = studentQuizService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var quizzes = await _studentQuizService.GetAvailableQuizzesAsync(user.Id);
            return View(quizzes);
        }

        [HttpGet]
        public async Task<IActionResult> Start(long quizId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();
                
                var attempt = await _studentQuizService.StartAttemptAsync(quizId, user.Id);
                return RedirectToAction("Attempt", new { id = attempt.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("AvailableQuizzes", "StudentDashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartPost(long quizId)
        {
            return await Start(quizId);
        }

        [HttpGet]
        public async Task<IActionResult> Attempt(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var attempt = await _studentQuizService.GetAttemptWithQuestionsAsync(id, user.Id);
            if (attempt == null) return NotFound();

            if (attempt.IsSubmitted)
            {
                return RedirectToAction("Result", new { id = attempt.Id });
            }

            return View(attempt);
        }

        [HttpPost]
        public async Task<IActionResult> Autosave(long attemptId, long questionId, string? selectedOption)
        {
            var user = await _userManager.GetUserAsync(User);
            var attempt = await _studentQuizService.GetAttemptWithQuestionsAsync(attemptId, user.Id);
            if (attempt == null) return Unauthorized();

            await _studentQuizService.AutosaveAnswerAsync(attemptId, questionId, selectedOption);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(long attemptId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();

                await _studentQuizService.SubmitAttemptAsync(attemptId, user.Id);
                return RedirectToAction("Result", new { id = attemptId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Attempt", new { id = attemptId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Result(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var attempt = await _studentQuizService.GetAttemptWithQuestionsAsync(id, user.Id);
            if (attempt == null || !attempt.IsSubmitted)
                return NotFound();

            return View(attempt);
        }
    }
}
