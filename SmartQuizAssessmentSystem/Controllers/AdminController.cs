using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartQuizAssessmentSystem.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Instructors()
        {
            return View();
        }

        public IActionResult Students()
        {
            return View();
        }

        public IActionResult Classes()
        {
            return View();
        }

        public IActionResult Subjects()
        {
            return View();
        }

        public IActionResult Quizzes()
        {
            return View();
        }

        public IActionResult Questions()
        {
            return View();
        }

        public IActionResult QuestionBank()
        {
            return View();
        }

        public IActionResult AssignInstructor()
        {
            return View();
        }
    }
}
