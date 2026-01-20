using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin,Instructor")]
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IEducationMediumService _mediumService;
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;
        private readonly IInstructorService _instructorService;

        public QuizController(
            IQuizService quizService, 
            UserManager<QuizSystemUser> userManager,
            IEducationMediumService mediumService,
            IClassService classService,
            ISubjectService subjectService,
            IInstructorService instructorService)
        {
            _quizService = quizService;
            _userManager = userManager;
            _mediumService = mediumService;
            _classService = classService;
            _subjectService = subjectService;
            _instructorService = instructorService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var quizzes = await _quizService.GetAllAsync(currentUser);
            return View(quizzes);
        }

        public async Task<IActionResult> Details(long id)
        {
            var quiz = await _quizService.GetEntityAsync(id, includeQuestions: true);
            if (quiz == null) return NotFound();
            return View(quiz);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new QuizViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _quizService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _quizService.GetForEditAsync(id);
            if (vm == null) return NotFound();

            await PopulateDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, QuizViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

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
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
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
        public async Task<IActionResult> Approve(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.ApproveAsync(id, currentUser!);
            if (!ok) return NotFound();
            return RedirectToAction(redirect);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.RejectAsync(id, currentUser!);
            if (!ok) return NotFound();
            return RedirectToAction(redirect);
        }
        [HttpGet]
        public async Task<JsonResult> GetClasses(long? mediumId)
        {
            var classes = (mediumId.HasValue && mediumId.Value > 0)
                ? await _classService.GetAllAsync(mediumId.Value)
                : await _classService.GetAllAsync(null);

            return Json(classes.Select(c => new { id = c.Id, name = c.Name }));
        }

        [HttpGet]
        public async Task<JsonResult> GetSubjects(long? classId)
        {
            var subjects = (classId.HasValue && classId.Value > 0)
                ? await _subjectService.GetAllAsync(classId.Value)
                : await _subjectService.GetAllAsync(null);

            return Json(subjects.Select(s => new { id = s.Id, name = s.Name }));
        }

        private async Task PopulateDropdownsAsync(long? mediumId = null, long? classId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(currentUser.Id);

            if (instructor != null && instructor.ClassId.HasValue)
            {
                
                mediumId = instructor.EducationMediumId;
                classId = instructor.ClassId;

                var medium = await _mediumService.GetByIdAsync(mediumId.Value);
                ViewBag.EducationMediumId = new SelectList(new[] { medium }, "Id", "Name", mediumId);

                var cls = await _classService.GetByIdAsync(classId.Value);
                ViewBag.ClassId = new SelectList(new[] { cls }, "Id", "Name", classId);
            }
            else
            {
                
                var mediums = await _mediumService.GetAllAsync();
                ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

                if (mediumId.HasValue)
                {
                    var classes = await _classService.GetAllAsync(mediumId.Value);
                    ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
                }
                else
                {
                    ViewBag.ClassId = new SelectList(Enumerable.Empty<Class>(), "Id", "Name");
                }
            }

            if (classId.HasValue && classId.Value > 0)
            {
                var subjects = await _subjectService.GetAllAsync(classId.Value);
                ViewBag.SubjectId = new SelectList(subjects, "Id", "Name");
            }
            else
            {
                var allSubjects = await _subjectService.GetAllAsync();
                ViewBag.SubjectId = new SelectList(allSubjects, "Id", "Name");
            }
        }
    }
}
