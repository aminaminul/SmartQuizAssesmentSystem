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

        public async Task<IActionResult> Index(long? selectedMediumId)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.Mediums = mediums;
            ViewBag.SelectedMediumId = selectedMediumId;

            var classes = new List<Class>();
            if (selectedMediumId.HasValue)
            {
                var enumId = (EducationMediums)selectedMediumId.Value;
                classes = await _mediumService.GetClassesByMediumAsync(enumId);
            }
            ViewBag.Classes = classes;

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateMediumEnumDropdown();
            return View(new EducationMedium());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long mediumEnumId, EducationMedium model)
        {
            if (!ModelState.IsValid)
            {
                PopulateMediumEnumDropdown((EducationMediums)mediumEnumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            model.Id = (EducationMediums)mediumEnumId;

            try
            {
                await _mediumService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateMediumEnumDropdown((EducationMediums)mediumEnumId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var enumId = (EducationMediums)id;
            var medium = await _mediumService.GetByIdAsync(enumId);
            if (medium == null) return NotFound();

            PopulateMediumEnumDropdown(medium.Id);
            return View(medium);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, long mediumEnumId, EducationMedium model)
        {
            var enumId = (EducationMediums)id;

            if (enumId != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateMediumEnumDropdown((EducationMediums)mediumEnumId);
                return View(model);
            }

            try
            {
                model.Id = (EducationMediums)mediumEnumId;
                var ok = await _mediumService.UpdateAsync(enumId, model);
                if (!ok) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateMediumEnumDropdown((EducationMediums)mediumEnumId);
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var enumId = (EducationMediums)id;
            var medium = await _mediumService.GetByIdAsync(enumId);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var enumId = (EducationMediums)id;
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.SoftDeleteAsync(enumId, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var enumId = (EducationMediums)id;
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.ApproveAsync(enumId, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var enumId = (EducationMediums)id;
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _mediumService.RejectAsync(enumId, currentUser!);
            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private void PopulateMediumEnumDropdown(EducationMediums? selected = null)
        {
            var items = Enum.GetValues(typeof(EducationMediums))
                .Cast<EducationMediums>()
                .Select(m => new SelectListItem
                {
                    Value = ((long)m).ToString(),
                    Text = m.ToString(),
                    Selected = selected.HasValue && selected.Value == m
                })
                .ToList();

            ViewBag.MediumEnums = items;
        }
    }
}
