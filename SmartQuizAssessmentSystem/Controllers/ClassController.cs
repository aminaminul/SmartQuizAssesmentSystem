using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;
using QuizSystemModel.BusinessRules;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassController : Controller
    {
        private readonly IClassService _classService;
        private readonly IEducationMediumService _mediumService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public ClassController(IClassService classService, IEducationMediumService mediumService, UserManager<QuizSystemUser> userManager)
        {
            _classService = classService;
            _mediumService = mediumService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(long? educationMediumId)
        {
            var classes = await _classService.GetAllAsync(educationMediumId);
            var mediums = await _mediumService.GetAllAsync();

            ViewBag.EducationMediumId = new SelectList(mediums ?? new List<EducationMedium>(), "Id", "Name", educationMediumId);
            ViewBag.SelectedMediumId = educationMediumId;
            return View(classes ?? new List<Class>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new ClassCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.ClassId, model.EducationMediumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            try
            {
                await _classService.CreateAsync(model.ClassId, model.EducationMediumId, currentUser);
                TempData["SuccessMessage"] = "Class created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(model.ClassId, model.EducationMediumId);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pending = await _classService.GetPendingAsync();
            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _classService.ApproveAsync(id, currentUser);
            if (!ok) return NotFound();

            return RedirectToAction(redirect);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _classService.RejectAsync(id, currentUser);
            if (!ok) return NotFound();

            return RedirectToAction(redirect);
        }

        private async Task PopulateDropdownsAsync(long? selectedClassId = null, long? selectedMediumId = null)
        {
            var classItems = Enum.GetValues(typeof(ClassNameEnum))
                .Cast<ClassNameEnum>()
                .Select(e => new SelectListItem 
                { 
                    Value = ((int)e).ToString(), 
                    Text = e.ToString(), 
                    Selected = selectedClassId == (int)e 
                })
                .ToList();
            ViewBag.ClassId = classItems;

            var mediums = await _mediumService.GetAllAsync() ?? new List<EducationMedium>();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);
        }
    }
}
