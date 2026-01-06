using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;
using QuizSystemService.Services;

[Authorize(Roles = "Student")]
public class StudentProfileController : Controller
{
    private readonly UserManager<QuizSystemUser> _userManager;
    private readonly IProfileUpdateService _profileService;
    private readonly IEducationMediumService _mediumService;
    private readonly IClassService _classService;

    public StudentProfileController(
        UserManager<QuizSystemUser> userManager,
        IProfileUpdateService profileService,
        IEducationMediumService mediumService,
        IClassService classService)
    {
        _userManager = userManager;
        _profileService = profileService;
        _mediumService = mediumService;
        _classService = classService;
    }

    [HttpGet]
    public async Task<IActionResult> ViewProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var vm = await _profileService.GetStudentProfileForEditAsync(user.Id);
        return View(vm);

    }

    [HttpGet]
    public async Task<IActionResult> UpdateProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var vm = await _profileService.GetStudentProfileForEditAsync(user.Id);
        return View(vm);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(StudentProfileUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _profileService.RequestStudentProfileUpdateAsync(model);
            TempData["SuccessMessage"] = "Profile update request submitted for admin approval.";
            return RedirectToAction(nameof(ViewProfile));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
    private async Task PopulateStudentDropdownsAsync(long? selectedMediumId = null, long? selectedClassId = null)
    {
        var mediums = await _mediumService.GetAllAsync();
        var classes = await _classService.GetAllAsync(null);

        ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);
        ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedClassId);
    }
}
