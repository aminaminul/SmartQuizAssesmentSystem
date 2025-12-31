using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassController : Controller
    {
        private readonly IClassService _classService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public ClassController(
            IClassService classService,
            UserManager<QuizSystemUser> userManager)
        {
            _classService = classService;
            _userManager = userManager;
        }

        // Index
        public async Task<IActionResult> Index(long? educationMediumId)
        {
            var classes = await _classService.GetAllAsync(educationMediumId);

            var mediums = await _classService.GetEducationMediumsAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            return View(classes);
        }

        // Create (GET)
        public async Task<IActionResult> Create()
        {
            await PopulateEducationMediumDropdownAsync();
            return View(new Class());
        }

        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Class model, long? educationMediumId)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                return View(model);
            }

            if (!educationMediumId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select an education medium.");
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _classService.CreateAsync(model, educationMediumId.Value, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                return View(model);
            }
        }

        // Edit (GET)
        public async Task<IActionResult> Edit(long id)
        {
            var cls = await _classService.GetByIdAsync(id);
            if (cls == null) return NotFound();

            return View(cls);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Class model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var ok = await _classService.UpdateAsync(id, model);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // Details
        public async Task<IActionResult> Details(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null) return NotFound();
            return View(cls);
        }

        // Delete (GET)
        public async Task<IActionResult> Delete(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null) return NotFound();
            return View(cls);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _classService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _classService.ApproveAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _classService.RejectAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEducationMediumDropdownAsync(long? selectedId = null)
        {
            var mediums = await _classService.GetEducationMediumsAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedId);
        }
    }
}
