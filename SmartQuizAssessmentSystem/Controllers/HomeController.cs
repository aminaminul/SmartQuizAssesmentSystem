using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystemService.Interfaces;
using QuizSystemModel.Models;
using System.Diagnostics;

namespace QuizSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;
        private readonly IStudentService _studentService;

        public HomeController(
            ILogger<HomeController> logger,
            IClassService classService,
            ISubjectService subjectService,
            IStudentService studentService)
        {
            _logger = logger;
            _classService = classService;
            _subjectService = subjectService;
            _studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ClassCount = (await _classService.GetAllAsync(null)).Count;
            ViewBag.SubjectCount = (await _subjectService.GetAllAsync(null)).Count;
            ViewBag.StudentCount = (await _studentService.GetAllAsync()).Count;
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(Roles = "Instructor")]
        public IActionResult Instructor()
        {
            return View();
        }
        [Authorize(Roles = "Student")]
        public IActionResult Student()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
