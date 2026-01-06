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

        // LIST + classes for selected medium
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

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            PopulateMediumEnumDropdown(null);
            return View(new EducationMedium());
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long? mediumEnumId)
        {
            if (!mediumEnumId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select an education medium.");
                ViewData["MediumError"] = "Please select an education medium.";
            }

            if (!ModelState.IsValid)
            {
                PopulateMediumEnumDropdown(
                    mediumEnumId.HasValue ? (EducationMediums?)((EducationMediums)mediumEnumId.Value) : null);
                return View(new EducationMedium());
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var enumId = (EducationMediums)mediumEnumId!.Value;

            var model = new EducationMedium
            {
                // Id এখন long হলে, enum value কে long এ convert করো
                Id = (long)enumId,
                Name = enumId.ToString(),
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                IsApproved = false,
                CreatedBy = currentUser
            };

            try
            {
                await _mediumService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateMediumEnumDropdown(enumId);
                return View(model);
            }
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null) return NotFound();

            return View(medium);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, EducationMedium model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

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

        // DELETE (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.SoftDeleteAsync(id, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // APPROVE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var ok = await _mediumService.ApproveAsync(id, currentUser);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // REJECT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var ok = await _mediumService.RejectAsync(id, currentUser);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }
        private void PopulateMediumEnumDropdown(EducationMediums? selected = null)
        {
            var items = Enum.GetValues(typeof(EducationMediums))
                .Cast<EducationMediums>()
                .Select(m => new SelectListItem
                {
                    Value = ((long)m).ToString(),      // dropdown value
                    Text = m.ToString(),              // dropdown text
                    Selected = selected.HasValue && selected.Value == m
                })
                .ToList();

            ViewBag.MediumEnums = items;
        }
    }
}