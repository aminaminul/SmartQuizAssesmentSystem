using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassController : Controller
    {
        private readonly IClassService _classService;
        private readonly IEducationMediumService _mediumService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public ClassController(
            IClassService classService,
            IEducationMediumService mediumService,
            UserManager<QuizSystemUser> userManager)
        {
            _classService = classService;
            _mediumService = mediumService;
            _userManager = userManager;
        }

        // LIST
        public async Task<IActionResult> Index(long? educationMediumId)
        {
            var classes = await _classService.GetAllAsync(educationMediumId);
            var mediums = await _mediumService.GetAllAsync();

            ViewBag.EducationMediumId = new SelectList(
                mediums,
                "Id",
                "Name",
                educationMediumId
            );

            ViewBag.SelectedMediumId = educationMediumId;
            return View(classes);
        }

        // CREATE (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateEducationMediumDropdownAsync(null);
            PopulateClassNameDropdown(null);
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long? educationMediumId, ClassNameEnum? className)
        {
            if (!educationMediumId.HasValue)
                ModelState.AddModelError(string.Empty, "Please select an education medium.");

            if (!className.HasValue)
                ModelState.AddModelError(string.Empty, "Please select a class name.");

            if (!ModelState.IsValid)
            {
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                PopulateClassNameDropdown(className);
                return View();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _classService.CreateAsync(className!.Value, educationMediumId!.Value, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                PopulateClassNameDropdown(className);
                return View();
            }
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: false);
            if (cls == null) return NotFound();

            await PopulateEducationMediumDropdownAsync(cls.EducationMediumId);
            PopulateClassNameDropdown(cls.ClassName);
            return View(cls);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id,
            long educationMediumId,
            ClassNameEnum className,
            Class model)
        {
            if (id != model.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                PopulateClassNameDropdown(className);
                return View(model);
            }

            try
            {
                var ok = await _classService.UpdateAsync(id, className, educationMediumId, model.Status);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                PopulateClassNameDropdown(className);
                return View(model);
            }
        }

        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null) return NotFound();
            return View(cls);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _classService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // PENDING LIST
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingClasses = await _classService.GetPendingAsync();
            return View(pendingClasses);
        }

        // APPROVE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var ok = await _classService.ApproveAsync(id, currentUser);
            if (!ok)
                return NotFound();

            TempData["SuccessMessage"] = "Class approved successfully.";
            return RedirectToAction(nameof(Pending));
        }

        // REJECT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var ok = await _classService.RejectAsync(id, currentUser);
            if (!ok)
                return NotFound();

            TempData["SuccessMessage"] = "Class rejected.";
            return RedirectToAction(nameof(Pending));
        }

        // DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null)
                return NotFound();

            return View(cls);
        }

        // ---------- helpers ----------

        private async Task PopulateEducationMediumDropdownAsync(long? selectedId = null)
        {
            var mediums = await _mediumService.GetAllAsync();

            ViewBag.EducationMediumId = new SelectList(
                mediums,
                "Id",
                "Name",
                selectedId
            );
        }

        private void PopulateClassNameDropdown(ClassNameEnum? selected = null)
        {
            var items = Enum.GetValues(typeof(ClassNameEnum))
                .Cast<ClassNameEnum>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = c.ToString(),
                    Selected = selected.HasValue && selected.Value == c
                })
                .ToList();

            ViewBag.ClassNames = items;
        }
    }
}
