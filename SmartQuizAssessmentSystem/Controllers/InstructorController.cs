using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;
using QuizSystemModel.Interfaces; 


namespace SmartQuizAssessmentSystem.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IInstructorService _instructorService;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IClassService _classService;

        public InstructorController(
            IInstructorService instructorService,
            UserManager<QuizSystemUser> userManager,
            IClassService classService)
        {
            _instructorService = instructorService;
            _userManager = userManager;
            _classService = classService;
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new InstructorAddViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
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
                
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }
        }

        
        public async Task<IActionResult> Edit(long id)
        {
            var instructor = await _instructorService.GetForEditAsync(id);
            if (instructor == null)
                return NotFound();

            await PopulateDropdownsAsync(instructor.EducationMediumId, instructor.ClassId);
            return View(instructor);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Instructor model, long? educationMediumId, long? classId)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(educationMediumId, classId);
                return View(model);
            }

            try
            {
                var ok = await _instructorService.UpdateAsync(id, model, educationMediumId, classId);
                if (!ok)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(educationMediumId, classId);
                return View(model);
            }
        }

        
        public async Task<IActionResult> Details(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        
        public async Task<IActionResult> Delete(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var pendingInstructors = await _instructorService.GetPendingAsync();
            return View(pendingInstructors);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _instructorService.ApproveAsync(id, currentUser);
            if (ok)
                TempData["SuccessMessage"] = "Instructor approved successfully.";
            else
                TempData["ErrorMessage"] = "Failed to approve instructor.";

            return RedirectToAction(redirect);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string redirect = "Index")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var ok = await _instructorService.RejectAsync(id, currentUser);
            if (ok)
                TempData["SuccessMessage"] = "Instructor rejected.";
            else
                TempData["ErrorMessage"] = "Failed to reject instructor.";

            return RedirectToAction(redirect);
        }

        [HttpGet]
        public async Task<JsonResult> GetClassesByMedium(long? mediumId)
        {
             var classes = await _classService.GetAllAsync(mediumId);
             return Json(classes.Select(c => new { id = c.Id, name = c.Name }));
        }

        private async Task PopulateDropdownsAsync(long? selectedMediumId = null, long? selectedClassId = null)
        {
            var mediums = await _instructorService.GetEducationMediumsAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);

            var classes = await _classService.GetAllAsync(selectedMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedClassId);
        }
    }
}
