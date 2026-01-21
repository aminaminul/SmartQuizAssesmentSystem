using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorDashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IInstructorService _instructorService;
        private readonly IClassService _classService;
        private readonly IEducationMediumService _mediumService;
        private readonly ISubjectService _subjectService;
        private readonly IProfileUpdateService _profileService;
        private readonly IInstructorDashboardService _dashboardService;
        private readonly UserManager<QuizSystemUser> _userManager;
        public InstructorDashboardController(
            IStudentService studentService,
            IInstructorService instructorService,
            IClassService classService,
            IEducationMediumService mediumService,
            ISubjectService subjectService,
            IProfileUpdateService profileService,
            IInstructorDashboardService dashboardService,
            UserManager<QuizSystemUser> userManager)
        {
            _studentService = studentService;
            _instructorService = instructorService;
            _classService = classService;
            _mediumService = mediumService;
            _subjectService = subjectService;
            _dashboardService = dashboardService;
            _userManager = userManager;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var model = await _dashboardService.GetDashboardAsync(user.Id);
            return View(model);
        }

        public async Task<IActionResult> Students(long? educationMediumId, long? classId)
        {
            var students = await _studentService.GetAllAsync(classId, educationMediumId);
            await PopulateInstructorDropdownsAsync(educationMediumId, classId);
            return View(students);
        }

        public async Task<IActionResult> StudentDetails(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Instructors(long? educationMediumId, long? classId)
        {
            var instructors = await _instructorService.GetAllAsync(educationMediumId, classId);
            await PopulateInstructorDropdownsAsync(educationMediumId, classId);
            return View(instructors);
        }

        public async Task<IActionResult> InstructorDetails(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        public async Task<IActionResult> Classes(long? educationMediumId)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            var classes = await _classService.GetAllAsync(educationMediumId);
            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null)
                return NotFound();

            return View(cls);
        }

        public async Task<IActionResult> EducationMediums(long? educationMediumId)
        {
            var allMediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(allMediums, "Id", "Name", educationMediumId);

            var filteredMediums = await _mediumService.GetAllAsync(educationMediumId);
            return View(filteredMediums);
        }

        public async Task<IActionResult> EducationMediumDetails(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        public async Task<IActionResult> Subjects(long? educationMediumId, long? classId)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            var classes = await _classService.GetAllAsync(educationMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", classId);

            var subjects = await _subjectService.GetAllAsync(classId, educationMediumId);
            return View(subjects);
        }

        public async Task<IActionResult> SubjectDetails(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        #region My Class Students Management
        public async Task<IActionResult> MyClassStudents()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);
            if (instructor == null || !instructor.ClassId.HasValue)
            {
                TempData["ErrorMessage"] = "You do not have an assigned class.";
                return RedirectToAction(nameof(Dashboard));
            }

            var students = await _studentService.GetAllAsync(instructor.ClassId, instructor.EducationMediumId);
            ViewBag.ClassName = instructor.Class?.Name ?? (await _classService.GetByIdAsync(instructor.ClassId.Value))?.Name;
            return View(students);
        }

        [HttpGet]
        public async Task<IActionResult> CreateStudent()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);
            if (instructor == null || !instructor.ClassId.HasValue)
            {
                TempData["ErrorMessage"] = "You do not have an assigned class to add students to.";
                return RedirectToAction(nameof(MyClassStudents));
            }

            var model = new StudentAddViewModel
            {
                EducationMediumId = instructor.EducationMediumId,
                ClassId = instructor.ClassId
            };
            
            ViewBag.MediumName = (await _mediumService.GetByIdAsync(model.EducationMediumId ?? 0))?.Name;
            ViewBag.ClassName = (await _classService.GetByIdAsync(model.ClassId ?? 0))?.Name;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(StudentAddViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);
            
            // Force the instructor's class and medium
            model.EducationMediumId = instructor?.EducationMediumId ?? 0;
            model.ClassId = instructor?.ClassId ?? 0;

            if (!ModelState.IsValid)
            {
                ViewBag.MediumName = (await _mediumService.GetByIdAsync(model.EducationMediumId ?? 0))?.Name;
                ViewBag.ClassName = (await _classService.GetByIdAsync(model.ClassId ?? 0))?.Name;
                return View(model);
            }

            try
            {
                await _studentService.CreateAsync(model, user!);
                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction(nameof(MyClassStudents));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.MediumName = (await _mediumService.GetByIdAsync(model.EducationMediumId ?? 0))?.Name;
                ViewBag.ClassName = (await _classService.GetByIdAsync(model.ClassId ?? 0))?.Name;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditStudent(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);

            if (student == null || student.ClassId != instructor?.ClassId)
                return NotFound();

            ViewBag.MediumName = (await _mediumService.GetByIdAsync(student.EducationMediumId ?? 0))?.Name;
            ViewBag.ClassName = (await _classService.GetByIdAsync(student.ClassId ?? 0))?.Name;
            
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(long id, Student model)
        {
            if (id != model.Id) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);

            // Re-fetch to verify ownership
            var existing = await _studentService.GetByIdAsync(id);
            if (existing == null || existing.ClassId != instructor?.ClassId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.MediumName = (await _mediumService.GetByIdAsync(existing.EducationMediumId ?? 0))?.Name;
                ViewBag.ClassName = (await _classService.GetByIdAsync(existing.ClassId ?? 0))?.Name;
                return View(model);
            }

            try
            {
                model.User = user; // Required by service
                await _studentService.UpdateAsync(id, model, instructor.EducationMediumId, instructor.ClassId);
                TempData["SuccessMessage"] = "Student updated successfully.";
                return RedirectToAction(nameof(MyClassStudents));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.MediumName = (await _mediumService.GetByIdAsync(existing.EducationMediumId ?? 0))?.Name;
                ViewBag.ClassName = (await _classService.GetByIdAsync(existing.ClassId ?? 0))?.Name;
                return View(model);
            }
        }

        public async Task<IActionResult> MyClassStudentDetails(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);

            if (student == null || student.ClassId != instructor?.ClassId)
                return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMyClassStudent(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorService.GetByUserIdAsync(user.Id);
            
            var student = await _studentService.GetByIdAsync(id);
            if (student == null || student.ClassId != instructor?.ClassId)
                return NotFound();

            await _studentService.SoftDeleteAsync(id, user!);
            TempData["SuccessMessage"] = "Student removed successfully.";
            return RedirectToAction(nameof(MyClassStudents));
        }
        #endregion
        
        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            await PopulateInstructorDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View("ViewProfile", vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _profileService.GetInstructorProfileForEditAsync(user.Id);
            await PopulateInstructorDropdownsAsync(vm.EducationMediumId, vm.ClassId);
            return View("UpdateProfile", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(InstructorProfileUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateInstructorDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }

            try
            {
                await _profileService.RequestInstructorProfileUpdateAsync(model);
                TempData["SuccessMessage"] = "Profile update request submitted for admin approval.";
                return RedirectToAction(nameof(ViewProfile));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopulateInstructorDropdownsAsync(model.EducationMediumId, model.ClassId);
                return View("UpdateProfile", model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var model = new ChangePasswordViewModel
            {
                Email = user.Email
            };

            return View("ChangePassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ChangePassword", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Email", "Email does not match your account.");
                return View("ChangePassword", model);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View("ChangePassword", model);
            }

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(ViewProfile));
        }
        [HttpGet]
        public async Task<JsonResult> GetClassesByMedium(long? mediumId)
        {
            var classes = await _classService.GetAllAsync(mediumId);
            return Json(classes.Select(c => new { id = c.Id, name = c.Name }));
        }

        private async Task PopulateInstructorDropdownsAsync(long? selectedMediumId = null, long? selectedClassId = null)
        {
            var mediums = await _mediumService.GetAllAsync();
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", selectedMediumId);

            var classes = await _classService.GetAllAsync(selectedMediumId);
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", selectedClassId);
        }
    }
}
