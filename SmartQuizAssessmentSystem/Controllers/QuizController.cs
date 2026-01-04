using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public QuizController(IQuizService quizService, UserManager<QuizSystemUser> userManager)
        {
            _quizService = quizService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizService.GetAllAsync();
            return View(quizzes);
        }

        public async Task<IActionResult> Details(long id)
        {
            var quiz = await _quizService.GetEntityAsync(id, includeQuestions: true);
            if (quiz == null) return NotFound();
            return View(quiz);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new QuizViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _quizService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
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
            var vm = await _quizService.GetForEditAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, QuizViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                var ok = await _quizService.UpdateAsync(id, model, currentUser!);
                if (!ok) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var quiz = await _quizService.GetEntityAsync(id);
            if (quiz == null) return NotFound();
            return View(quiz);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingQuizzes = await _quizService.GetPendingAsync();
            return View(pendingQuizzes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.ApproveAsync(id, currentUser!);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.RejectAsync(id, currentUser!);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
