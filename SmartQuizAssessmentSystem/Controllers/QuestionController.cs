using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly ISubjectService _subjectService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public QuestionController(
            IQuestionService questionService,
            IQuizService quizService,
            ISubjectService subjectService,
            UserManager<QuizSystemUser> userManager)
        {
            _questionService = questionService;
            _quizService = quizService;
            _subjectService = subjectService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(long? quizId, string? subject)
        {
            Quiz? quiz = null;
            if (quizId.HasValue && quizId.Value > 0)
            {
                quiz = await _quizService.GetEntityAsync(quizId.Value);
            }

            // Get all active subjects from database
            var subjects = await _subjectService.GetAllAsync();
            var activeSubjects = subjects.Where(s => s.Status == QuizSystemModel.BusinessRules.ModelStatus.Active).ToList();

            var allQuizzes = await _quizService.GetAllAsync();
            var activeQuizzes = allQuizzes.Where(q => q.Status != QuizSystemModel.BusinessRules.ModelStatus.Deleted).OrderByDescending(q => q.Id).ToList();

            ViewBag.Quiz = quiz;
            ViewBag.QuizId = quizId;
            ViewBag.Subject = subject;
            ViewBag.Subjects = activeSubjects;
            ViewBag.Quizzes = activeQuizzes;

            var questions = await _questionService.GetAllAsync(quizId, subject);
            return View(questions); 
        }

        [HttpGet]
        public async Task<IActionResult> Create(long? quizId, string? subject, int count = 1)
        {
            var allQuizzes = await _quizService.GetAllAsync();
            var activeQuizzes = allQuizzes.Where(q => q.Status != QuizSystemModel.BusinessRules.ModelStatus.Deleted).OrderByDescending(q => q.Id).ToList();

            Quiz? quiz = null;
            if (quizId.HasValue && quizId.Value > 0)
            {
                quiz = await _quizService.GetEntityAsync(quizId.Value);
            }
            else if (activeQuizzes.Any())
            {
                quiz = activeQuizzes.FirstOrDefault();
            }

            var subjects = await _subjectService.GetAllAsync();
            var activeSubjects = subjects.Where(s => s.Status == QuizSystemModel.BusinessRules.ModelStatus.Active).ToList();

            if (count < 1) count = 1;
            if (count > 50) count = 50;

            var targetQuizId = quiz?.Id ?? (quizId ?? (activeQuizzes.FirstOrDefault()?.Id ?? 0));
            var targetSubject = quiz?.Subject?.Name ?? subject ?? activeSubjects.FirstOrDefault()?.Name ?? "General";

            var vm = new BatchQuestionViewModel
            {
                QuizId = targetQuizId,
                Subject = targetSubject,
                QuestionCount = count,
                Questions = new List<QuestionViewModel>()
            };

            for (int i = 0; i < count; i++)
            {
                vm.Questions.Add(new QuestionViewModel
                {
                    QuizId = targetQuizId,
                    Subject = targetSubject,
                    Marks = 1,
                    RightOption = "A"
                });
            }

            ViewBag.Quiz = quiz;
            ViewBag.QuizzesList = activeQuizzes;
            ViewBag.Quizzes = new SelectList(activeQuizzes, "Id", "Name", targetQuizId);
            ViewBag.Subjects = activeSubjects;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BatchQuestionViewModel model)
        {
            var allQuizzes = await _quizService.GetAllAsync();
            var activeQuizzes = allQuizzes.Where(q => q.Status != QuizSystemModel.BusinessRules.ModelStatus.Deleted).OrderByDescending(q => q.Id).ToList();

            var subjects = await _subjectService.GetAllAsync();
            var activeSubjects = subjects.Where(s => s.Status == QuizSystemModel.BusinessRules.ModelStatus.Active).ToList();

            var quiz = await _quizService.GetEntityAsync(model.QuizId);
            ViewBag.Quiz = quiz;
            ViewBag.QuizzesList = activeQuizzes;
            ViewBag.Quizzes = new SelectList(activeQuizzes, "Id", "Name", model.QuizId);
            ViewBag.Subjects = activeSubjects;

            if (quiz == null)
            {
                ModelState.AddModelError("QuizId", "Quiz not found.");
                return View(model);
            }

            // Automatically set subject from Quiz
            if (!string.IsNullOrWhiteSpace(quiz.Subject?.Name))
            {
                model.Subject = quiz.Subject.Name;
            }

            if (model.Questions == null || !model.Questions.Any())
            {
                ModelState.AddModelError(string.Empty, "Please provide at least one question.");
                return View(model);
            }

            // Ensure every question has the quiz's subject
            foreach (var q in model.Questions)
            {
                q.QuizId = model.QuizId;
                q.Subject = model.Subject;
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                var count = await _questionService.CreateBatchAsync(model, currentUser!);
                TempData["SuccessMessage"] = $"{count} question(s) added successfully to '{quiz.Name}'!";
                return RedirectToAction(nameof(Index), new { quizId = model.QuizId, subject = model.Subject });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
