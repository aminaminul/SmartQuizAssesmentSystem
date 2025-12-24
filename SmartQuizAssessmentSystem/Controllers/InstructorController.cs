using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize (Roles = "Instructor")]
    public class InstructorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
