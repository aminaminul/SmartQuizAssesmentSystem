using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize]
    public class PerformanceController : Controller
    {
        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult Student()
        {
            ViewData["Title"] = "Student Performance";
            return View();
        }

        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult Class()
        {
            ViewData["Title"] = "Class Performance";
            return View();
        }

        [Authorize(Roles = "Admin,Instructor")]
        public IActionResult EducationMedium()
        {
            ViewData["Title"] = "Education Medium Performance";
            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult MyPerformance()
        {
            ViewData["Title"] = "My Performance";
            return View();
        }
    }
}
