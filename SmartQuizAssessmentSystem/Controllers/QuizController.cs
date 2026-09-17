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

        public async Task<IActionResult> Index(long? educationMediumId, long? classId, long? subjectId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var quizzes = await _quizService.GetAllAsync(currentUser, educationMediumId, classId, subjectId);
            await PopulateDropdownsAsync(educationMediumId, classId, subjectId);
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
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId, model.SubjectId);
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
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId, model.SubjectId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var vm = await _quizService.GetForEditAsync(id);
            if (vm == null) return NotFound();

            await PopulateDropdownsAsync(vm.EducationMediumId, vm.ClassId, vm.SubjectId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, QuizViewModel model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId, model.SubjectId);
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
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId, model.SubjectId);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pendingQuizzes = await _quizService.GetPendingAsync();
            return View(pendingQuizzes);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.ApproveAsync(id, currentUser!);
            if (!ok) return NotFound();
            var target = (redirect == "Pending") ? "Pending" : "Index";
            return RedirectToAction(target);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _quizService.RejectAsync(id, currentUser!);
            if (!ok) return NotFound();
            var target = (redirect == "Pending") ? "Pending" : "Index";
            return RedirectToAction(target);
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

        private async Task PopulateDropdownsAsync(long? mediumId = null, long? classId = null, long? subjectId = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var instructor = currentUser != null ? await _instructorService.GetByUserIdAsync(currentUser.Id) : null;

            if (instructor != null)
            {
                // --- Instructor view: subject is FIXED, class is free within their medium ---
                ViewBag.IsInstructor = true;

                // Medium: fixed to instructor's medium
                var mediumIdToUse = instructor.EducationMediumId;
                var medium = mediumIdToUse.HasValue ? await _mediumService.GetByIdAsync(mediumIdToUse.Value) : null;
                ViewBag.EducationMediumId = new SelectList(
                    medium != null ? new[] { medium } : Enumerable.Empty<EducationMedium>(),
                    "Id", "Name", mediumIdToUse);

                // Class: all classes within the instructor's medium (instructor picks any)
                var classes = mediumIdToUse.HasValue
                    ? await _classService.GetAllAsync(mediumIdToUse.Value)
                    : await _classService.GetAllAsync(null);
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId ?? instructor.ClassId);

                // Subject: FIXED to instructor's assigned subject only
                var subjectIdToUse = instructor.SubjectId;
                var subject = subjectIdToUse.HasValue ? await _subjectService.GetByIdAsync(subjectIdToUse.Value) : null;
                ViewBag.SubjectId = new SelectList(
                    subject != null ? new[] { subject } : Enumerable.Empty<Subject>(),
                    "Id", "Name", subjectIdToUse);
                ViewBag.FixedSubjectId = subjectIdToUse;
            }
            else
            {
                // --- Admin view: full unrestricted dropdowns ---
                ViewBag.IsInstructor = false;

                var mediums = await _mediumService.GetAllAsync();
                ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", mediumId);

                if (mediumId.HasValue && mediumId.Value > 0)
                {
                    var classes = await _classService.GetAllAsync(mediumId.Value);
                    ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
                }
                else
                {
                    var allClasses = await _classService.GetAllAsync(null);
                    ViewBag.ClassId = new SelectList(allClasses, "Id", "Name", classId);
                }

                if (classId.HasValue && classId.Value > 0)
                {
                    var subjects = await _subjectService.GetAllAsync(classId.Value);
                    ViewBag.SubjectId = new SelectList(subjects, "Id", "Name", subjectId);
                }
                else
                {
                    var allSubjects = await _subjectService.GetAllAsync();
                    ViewBag.SubjectId = new SelectList(allSubjects, "Id", "Name", subjectId);
                }
            }
        }
    }
}
