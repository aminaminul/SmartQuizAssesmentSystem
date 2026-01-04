using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
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
        private readonly IInstructorDashboardService _dashboardService;
        private readonly UserManager<QuizSystemUser> _userManager;
        public InstructorDashboardController(
            IStudentService studentService,
            IInstructorService instructorService,
            IClassService classService,
            IEducationMediumService mediumService,
            ISubjectService subjectService,
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

        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllAsync();
            return View(students);
        }

        public async Task<IActionResult> StudentDetails(long id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound();

            return View(student);
        }

        public async Task<IActionResult> Instructors()
        {
            var instructors = await _instructorService.GetAllAsync();
            return View(instructors);
        }

        public async Task<IActionResult> InstructorDetails(long id)
        {
            var instructor = await _instructorService.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        public async Task<IActionResult> Classes()
        {
            var classes = await _classService.GetAllAsync(null);
            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(long id)
        {
            var cls = await _classService.GetByIdAsync(id, includeMedium: true);
            if (cls == null)
                return NotFound();

            return View(cls);
        }

        public async Task<IActionResult> EducationMediums()
        {
            var mediums = await _mediumService.GetAllAsync();
            return View(mediums);
        }

        public async Task<IActionResult> EducationMediumDetails(long id)
        {
            var medium = await _mediumService.GetByIdAsync(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        public async Task<IActionResult> Subjects()
        {
            var subjects = await _subjectService.GetAllAsync();
            return View(subjects);
        }

        public async Task<IActionResult> SubjectDetails(long id)
        {
            var subject = await _subjectService.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }
    }
}
