using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;
using QuizSystemService.Services;

namespace SmartQuizAssessmentSystem.Controllers
{
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(long? educationMediumId)
        {
            var classes = await _classService.GetAllAsync(educationMediumId);
            var mediums = await _mediumService.GetAllAsync();

            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            return View(classes);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateEducationMediumDropdownAsync();
            return View(new Class());
        }

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

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var cls = await _classService.GetByIdAsync(id);
            if (cls == null) return NotFound();

            return View(cls);
        }

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

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _classService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEducationMediumDropdownAsync(long? selectedId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedId);
        }
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
    }
}
