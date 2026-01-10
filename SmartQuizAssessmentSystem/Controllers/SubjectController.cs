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
            var vm = new SubjectCreateViewModel
            {
                EducationMediumList = GetEducationMediumSelectList(),
                ClassList = Enumerable.Empty<SelectListItem>()
            };
            return View(vm);
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.EducationMediumList = GetEducationMediumSelectList();
                vm.ClassList = await GetClassSelectListAsync(vm.EducationMedium, vm.ClassId);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var subject = new Subject
            {
                Name = vm.Name,
                ClassId = vm.ClassId,
                IsApproved = vm.IsApproved,
                Status = ModelStatus.Active
            };

            try
            {
                await _subjectService.CreateAsync(subject, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.EducationMediumList = GetEducationMediumSelectList();
                vm.ClassList = await GetClassSelectListAsync(vm.EducationMedium, vm.ClassId);
                return View(vm);
            }
        }
        // GET: Subject/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id, includeClass: true);
            if (subject == null) return NotFound();

            EducationMediums? medium = null;

            if (subject.Class != null && subject.Class.EducationMediumId.HasValue)
            {
                medium = (EducationMediums)subject.Class.EducationMediumId.Value;
            }

            var vm = new SubjectCreateViewModel
            {
                Id = subject.Id,
                Name = subject.Name,
                ClassId = subject.ClassId,
                IsApproved = subject.IsApproved,
                EducationMedium = medium,
                EducationMediumList = GetEducationMediumSelectList(),
                ClassList = await GetClassSelectListAsync(medium, subject.ClassId)
            };

            return View(vm);
        }


        // POST: Subject Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SubjectCreateViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.EducationMediumList = GetEducationMediumSelectList();
                vm.ClassList = await GetClassSelectListAsync(vm.EducationMedium, vm.ClassId);
                return View(vm);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var subject = new Subject
            {
                Id = vm.Id!.Value,
                Name = vm.Name,
                ClassId = vm.ClassId,
                IsApproved = vm.IsApproved,
                Status = ModelStatus.Active
            };

            try
            {
                var ok = await _subjectService.UpdateAsync(id, subject, currentUser!);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                vm.EducationMediumList = GetEducationMediumSelectList();
                vm.ClassList = await GetClassSelectListAsync(vm.EducationMedium, vm.ClassId);
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
            var ok = await _subjectService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
        // Pending list
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingSubjects = await _subjectService.GetPendingAsync();
            return View(pendingSubjects);
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

        private async Task PopulateClassDropdownAsync(long? selectedId = null)
        {
            var classes = await _classService.GetAllAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedId);
        }

        private IEnumerable<SelectListItem> GetEducationMediumSelectList()
        {
            return Enum.GetValues(typeof(EducationMediums))
                .Cast<EducationMediums>()
                .Select(m => new SelectListItem
                {
                    Value = m.ToString(),
                    Text = m.ToString()
                });
        }

        private async Task<IEnumerable<SelectListItem>> GetClassSelectListAsync(
            EducationMediums? selectedMedium,
            long? selectedClassId)
        {
            if (!selectedMedium.HasValue)
                return Enumerable.Empty<SelectListItem>();

            var classes = await _classService.GetByMediumEnumAsync(selectedMedium.Value);

            return classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.ClassName.ToString(),
                Selected = selectedClassId.HasValue && selectedClassId.Value == c.Id
            });
        }

    }
}
