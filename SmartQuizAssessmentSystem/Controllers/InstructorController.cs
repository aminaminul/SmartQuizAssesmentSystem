using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly IInstructorService _instructorService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public InstructorController(
            IInstructorService instructorService,
            UserManager<QuizSystemUser> userManager)
        {
            _instructorService = instructorService;
            _userManager = userManager;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        // CREATE (GET)
        public async Task<IActionResult> Create()
        {
            await PopulateEducationMediumDropdownAsync();
            return View(new InstructorAddViewModel());
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEducationMediumDropdownAsync(model.EducationMediumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _instructorService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // any business error from service (email/phone duplicate, identity errors) 
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEducationMediumDropdownAsync(model.EducationMediumId);
                return View(model);
            }
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(long id)
        {
            var instructor = await _instructorService.GetForEditAsync(id);
            if (instructor == null)
                return NotFound();

            await PopulateEducationMediumDropdownAsync(instructor.EducationMediumId);
            return View(instructor);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Instructor model, long? educationMediumId)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                return View(model);
            }

            try
            {
                var ok = await _instructorService.UpdateAsync(id, model, educationMediumId);
                if (!ok)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateEducationMediumDropdownAsync(educationMediumId);
                return View(model);
            }
        }

        // DETAILS
        public async Task<IActionResult> Details(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _instructorService.SoftDeleteAsync(id, currentUser!);
            if (!ok)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateEducationMediumDropdownAsync(long? selectedId = null)
        {
            var mediums = await _instructorService.GetEducationMediumsAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedId);
        }
    }
}
