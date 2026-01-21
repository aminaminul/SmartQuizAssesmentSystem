using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public StudentController(
            IStudentService studentService,
            UserManager<QuizSystemUser> userManager)
        {
            _studentService = studentService;
            _userManager = userManager;
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(long? classId, long? educationMediumId)
        {
            var students = await _studentService.GetAllAsync(classId, educationMediumId);
            
            var mediums = await _studentService.GetEducationMediumsAsync();
            var classes = await _studentService.GetClassesAsync(educationMediumId);

            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedMediumId = educationMediumId;
            
            if (classId.HasValue)
            {
                var cls = classes.FirstOrDefault(c => c.Id == classId.Value);
                ViewBag.ClassName = cls?.Name;
            }

            return View(students ?? new List<Student>());
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View(new StudentAddViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StudentAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _studentService.CreateAsync(model, currentUser);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View(model);
            }
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id)
        {
            var student = await _studentService.GetForEditAsync(id);
            if (student == null)
                return NotFound();

            await PopulateDropdownsAsync(student.EducationMediumId, student.ClassId);
            return View(student);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id, Student model, long? educationMediumId, long? classId)
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
                var ok = await _studentService.UpdateAsync(id, model, educationMediumId, classId);
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
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id, long? classId)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            ViewBag.SelectedClassId = classId;
            return View(student);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id, long? classId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ok = await _studentService.SoftDeleteAsync(id, currentUser);
            if (!ok)
                return NotFound();

            if (classId.HasValue)
            {
                return RedirectToAction(nameof(Index), new { classId = classId.Value });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(long? selectedEducationMediumId = null, long? selectedClassId = null)
        {
            var educationMediums = await _studentService.GetEducationMediumsAsync() ?? new List<EducationMedium>();
            var classes = await _studentService.GetClassesAsync(selectedEducationMediumId) ?? new List<Class>();

            ViewBag.EducationMediumId = new SelectList(educationMediums.Any() ? educationMediums : new List<EducationMedium> { new EducationMedium { Id = 0, Name = "No Mediums Available" } }, "Id", "Name", selectedEducationMediumId);
            ViewBag.ClassId = new SelectList(classes.Any() ? classes : new List<Class> { new Class { Id = 0, Name = "No Classes Available" } }, "Id", "Name", selectedClassId);
        }
    }
}
