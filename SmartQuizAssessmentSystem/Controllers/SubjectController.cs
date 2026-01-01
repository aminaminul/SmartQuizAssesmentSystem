using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly IClassService _classService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public SubjectController(
            ISubjectService subjectService,
            IClassService classService,
            UserManager<QuizSystemUser> userManager)
        {
            _subjectService = subjectService;
            _classService = classService;
            _userManager = userManager;
        }

        // GET: Subject
        public async Task<IActionResult> Index(long? classId)
        {
            var subjects = await _subjectService.GetAllAsync(classId);
            var classes = await _classService.GetAllAsync(); // or a lighter call if you have one

            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
            return View(subjects);
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        // GET: Subject/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateClassDropdownAsync();
            return View(new Subject());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject model, long? classId)
        {
            if (!ModelState.IsValid)
            {
                await PopulateClassDropdownAsync(classId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _subjectService.CreateAsync(model, classId, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateClassDropdownAsync(classId);
                return View(model);
            }
        }

        // GET: Subject/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null)
                return NotFound();

            await PopulateClassDropdownAsync(subject.ClassId);
            return View(subject);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Subject model, long? classId)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                await PopulateClassDropdownAsync(classId ?? model.ClassId);
                return View(model);
            }

            try
            {
                var ok = await _subjectService.UpdateAsync(id, model, classId);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateClassDropdownAsync(classId ?? model.ClassId);
                return View(model);
            }
        }

        // GET: Subject/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _subjectService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateClassDropdownAsync(long? selectedId = null)
        {
            var classes = await _classService.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedId);
        }
    }
}
