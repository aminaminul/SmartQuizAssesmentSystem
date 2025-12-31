using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IQuizService _quizService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public QuestionController(
            IQuestionService questionService,
            IQuizService quizService,
            UserManager<QuizSystemUser> userManager)
        {
            _questionService = questionService;
            _quizService = quizService;
            _userManager = userManager;
        }

        // List questions of a quiz, optionally by subject
        public async Task<IActionResult> Index(long quizId, string? subject)
        {
            var quiz = await _quizService.GetByIdAsync(quizId);
            if (quiz == null) return NotFound();

            ViewBag.Quiz = quiz;
            ViewBag.Subject = subject;

            var questions = await _questionService.GetByQuizAsync(quizId, subject);
            return View(questions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(long quizId, string? subject)
        {
            var quiz = await _quizService.GetByIdAsync(quizId);
            if (quiz == null) return NotFound();

            var model = new QuestionBank
            {
                QuizId = quizId,
                Subject = subject ?? quiz.Subject
            };

            ViewBag.Quiz = quiz;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionBank model)
        {
            var quiz = await _quizService.GetByIdAsync(model.QuizId);
            if (quiz == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Quiz = quiz;
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _questionService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index), new { quizId = model.QuizId, subject = model.Subject });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Quiz = quiz;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();

            var quiz = await _quizService.GetByIdAsync(question.QuizId);
            ViewBag.Quiz = quiz;

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, QuestionBank model)
        {
            if (id != model.Id) return NotFound();

            var quiz = await _quizService.GetByIdAsync(model.QuizId);
            if (quiz == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Quiz = quiz;
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                var ok = await _questionService.UpdateAsync(id, model, currentUser!);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index), new { quizId = model.QuizId, subject = model.Subject });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Quiz = quiz;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();

            var quiz = await _quizService.GetByIdAsync(question.QuizId);
            ViewBag.Quiz = quiz;

            return View(question);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _questionService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index), new { quizId = question.QuizId, subject = question.Subject });
        }
    }
}
