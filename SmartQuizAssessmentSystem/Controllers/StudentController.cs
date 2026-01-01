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

        public StudentController(IStudentService studentService,UserManager<QuizSystemUser> userManager)
        {
            _studentService = studentService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Student")]
        public IActionResult Dashboard()
        {
            return View();
        }

        // LIST
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        // DETAILS
        public async Task<IActionResult> Details(long id)
        {
            var student = await _studentService.GetByIdAsync(id, includeUser: true);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // CREATE (GET)
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            var vm = new StudentAddView { Role = "Student" };
            return View(vm);
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentAddView model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model.EducationMediumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                await _studentService.CreateAsync(model, currentUser!);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateDropdownsAsync(model.EducationMediumId);
                return View(model);
            }
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            await PopulateDropdownsAsync(student.EducationMediumId, student.ClassId);
            return View(student);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // DELETE (GET)
        public async Task<IActionResult> Delete(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var ok = await _studentService.SoftDeleteAsync(id, currentUser!);
            if (!ok)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropdownsAsync(long? educationMediumId = null, long? classId = null)
        {
            var mediums = await _studentService.GetEducationMediumsAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            var classes = await _studentService.GetClassesAsync();
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);
        }
    }
}
