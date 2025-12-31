using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EducationMediumController : Controller
    {
        private readonly IEducationMediumService _mediumService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public EducationMediumController(
            IEducationMediumService mediumService,
            UserManager<QuizSystemUser> userManager)
        {
            _mediumService = mediumService;
            _userManager = userManager;
        }

        // Index
        public async Task<IActionResult> Index(long? selectedMediumId)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.Mediums = mediums;
            ViewBag.SelectedMediumId = selectedMediumId;

            var classes = new List<Class>();
            if (selectedMediumId.HasValue)
            {
                classes = await _mediumService.GetClassesByMediumAsync(selectedMediumId.Value);
            }
            ViewBag.Classes = classes;

            return View();
        }

        // Create (GET)
        public IActionResult Create()
        {
            return View(new EducationMedium());
        }

        // Create (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EducationMedium model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _mediumService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // Edit (GET)
        public async Task<IActionResult> Edit(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, EducationMedium model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var ok = await _mediumService.UpdateAsync(id, model);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // Delete (GET)
        public async Task<IActionResult> Delete(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.ApproveAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.RejectAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
