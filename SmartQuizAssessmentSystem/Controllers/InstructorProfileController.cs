using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorProfileController : Controller
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IProfileUpdateService _profileService;
        private readonly IEducationMediumService _mediumService;
        public InstructorProfileController(
            UserManager<QuizSystemUser> userManager,
            IProfileUpdateService profileService,
            IEducationMediumService mediumService)

        {
            _userManager = userManager;
            _profileService = profileService;
            _mediumService = mediumService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(InstructorProfileUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _profileService.RequestInstructorProfileUpdateAsync(model);
                TempData["SuccessMessage"] = "Profile update request submitted for admin approval.";
                return RedirectToAction(nameof(ViewProfile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
        private async Task PopulateInstructorDropdownsAsync(long? selectedMediumId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);
        }

    }
}
