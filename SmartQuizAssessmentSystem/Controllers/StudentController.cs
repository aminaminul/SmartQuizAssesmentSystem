using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartQuizAssessmentSystem.Controllers
{ 
    public class StudentController : Controller
    {
        [Authorize(Roles = "Student")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
    
}
