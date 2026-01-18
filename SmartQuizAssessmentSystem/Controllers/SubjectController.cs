using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;
using SmartQuizAssessmentSystem.ViewModels;

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

        // GET: Subject/Index
        public async Task<IActionResult> Index(long? classId)
        {
            var subjects = await _subjectService.GetAllAsync(classId);
            var classes = await _classService.GetAllAsync();

            ViewBag.ClassId = new SelectList(classes ?? new List<Class>(), "Id", "Name", classId);
            return View(subjects ?? new List<Subject>());
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null) return NotFound();
            return View(subject);
        }

        // GET: Subject/Create ✅ SINGLE ViewModel
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new SubjectViewModel
            {
                EducationMediumList = GetEducationMediumSelectList()
            };
            await PopulateClassListAsync(vm);
            return View(vm);
        }

        // POST: Subject/Create ✅ SINGLE ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.EducationMediumList = GetEducationMediumSelectList();
                await PopulateClassListAsync(vm);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            try
            {
                var subject = new Subject
                {
                    Name = vm.Name,
                    ClassId = vm.ClassId,
                    IsApproved = vm.IsApproved,
                    Status = ModelStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUser
                };

                await _subjectService.CreateAsync(subject, currentUser);
                TempData["SuccessMessage"] = "Subject created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.EducationMediumList = GetEducationMediumSelectList();
                await PopulateClassListAsync(vm);
                return View(vm);
            }
        }

        // GET: Subject/Edit/5 ✅ SINGLE ViewModel
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null || !subject.ClassId.HasValue) return NotFound();

            var cls = await _classService.GetByIdAsync(subject.ClassId.Value, true);
            if (cls == null) return NotFound();

            var vm = new SubjectViewModel
            {
                Id = subject.Id,
                Name = subject.Name,
                ClassId = subject.ClassId!.Value,
                IsApproved = subject.IsApproved,
                EducationMediumId = cls.EducationMediumId,
                Status = subject.Status,
                EducationMediumList = GetEducationMediumSelectList(),
                Class = cls
            };

            await PopulateClassListAsync(vm);
            return View(vm);
        }

        // POST: Subject/Edit/5 ✅ SINGLE ViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel vm)
        {
            if (!vm.Id.HasValue || vm.Id <= 0) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.EducationMediumList = GetEducationMediumSelectList();
                await PopulateClassListAsync(vm);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            try
            {
                var subject = new Subject
                {
                    Id = vm.Id.Value,
                    Name = vm.Name,
                    ClassId = vm.ClassId,
                    IsApproved = vm.IsApproved,
                    Status = vm.Status
                };

                var ok = await _subjectService.UpdateAsync(vm.Id.Value, subject, currentUser);
                if (!ok) return NotFound();

                TempData["SuccessMessage"] = "Subject updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.EducationMediumList = GetEducationMediumSelectList();
                await PopulateClassListAsync(vm);
                return View(vm);
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
            if (currentUser == null) return Unauthorized();

            var ok = await _subjectService.SoftDeleteAsync(id, currentUser);
            if (!ok) return NotFound();

            TempData["SuccessMessage"] = "Subject deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Pending list
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingSubjects = await _subjectService.GetPendingAsync();
            return View(pendingSubjects ?? new List<Subject>());
        }

        // Approve subject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _subjectService.ApproveAsync(id, currentUser);
            if (!ok) return NotFound();

            TempData["SuccessMessage"] = "Subject approved successfully.";
            return RedirectToAction(nameof(Pending));
        }

        // Reject subject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _subjectService.RejectAsync(id, currentUser);
            if (!ok) return NotFound();

            TempData["SuccessMessage"] = "Subject rejected.";
            return RedirectToAction(nameof(Pending));
        }


        private IEnumerable<SelectListItem> GetEducationMediumSelectList()
        {
            return Enum.GetValues<EducationMediums>()
                .Select(m => new SelectListItem
                {
                    Value = ((long)m).ToString(),
                    Text = m.ToString()
                });
        }

        private async Task PopulateClassListAsync(SubjectViewModel vm)
        {
            if (vm.EducationMediumId.HasValue)
            {
                var classes = await _classService.GetAllAsync(vm.EducationMediumId);
                vm.ClassList = classes.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = vm.ClassId == c.Id
                });
            }
        }
    }
}
