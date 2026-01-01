using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
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

        public async Task<IActionResult> Index(long quizId, string? subject)
        {
            var quiz = await _quizService.GetEntityAsync(quizId);
            if (quiz == null) return NotFound();

            ViewBag.Quiz = quiz;
            ViewBag.Subject = subject;

            var questions = await _questionService.GetByQuizAsync(quizId, subject);
            return View(questions); // entity list
        }

        [HttpGet]
        public async Task<IActionResult> Create(long quizId, string? subject)
        {
            var quiz = await _quizService.GetEntityAsync(quizId);
            if (quiz == null) return NotFound();

            ViewBag.Quiz = quiz;

            var vm = new QuestionViewModel
            {
                QuizId = quizId,
                Subject = subject ?? quiz.Subject
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel model)
        {
            var quiz = await _quizService.GetEntityAsync(model.QuizId);
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
            var vm = await _questionService.GetForEditAsync(id);
            if (vm == null) return NotFound();

            var quiz = await _quizService.GetEntityAsync(vm.QuizId);
            ViewBag.Quiz = quiz;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, QuestionViewModel model)
        {
            if (id != model.Id) return NotFound();

            var quiz = await _quizService.GetEntityAsync(model.QuizId);
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
            var q = await _questionService.GetEntityAsync(id);
            if (q == null) return NotFound();

            var quiz = await _quizService.GetEntityAsync(q.QuizId);
            ViewBag.Quiz = quiz;

            return View(q);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var q = await _questionService.GetEntityAsync(id);
            if (q == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _questionService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index), new { quizId = q.QuizId, subject = q.Subject });
        }
    }
}
