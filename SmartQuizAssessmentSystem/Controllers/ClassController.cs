using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.ViewModels;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;

        public ClassController(AppDbContext context, UserManager<QuizSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Class Index
        public IActionResult Index(long? educationMediumId)
        {
            var classes = _context.Class
                .Include(c => c.EducationMedium)
                .AsQueryable();

            if (educationMediumId.HasValue)
            {
                classes = classes.Where(c => c.EducationMedium != null &&
                                             c.EducationMedium.Id == educationMediumId.Value);
            }

            var mediums = _context.EducationMedium.ToList();
            ViewBag.EducationMediumId = new SelectList(
                mediums, "Id", "Name", educationMediumId);

            return View(classes.ToList());
        }

        //Class Create
        public IActionResult Create()
        {
            var vm = new ClassMediumView();
            return View(vm);
        }

        //Class Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClassMediumView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = _userManager.GetUserAsync(User).Result;

            var className = model.Class.ToString(); 
            var mediumName = model.Medium.ToString();

            var medium = _context.EducationMedium
                .FirstOrDefault(m => m.Name.ToLower() == mediumName.ToLower());

            if (medium == null)
            {
                ModelState.AddModelError("Medium", "Selected Education Medium Does Not Exist. Please Create It First.");
                return View(model);
            }

            // Prevent Duplicate Class Under Same Medium
            bool exists = _context.Class
                .Include(c => c.EducationMedium)
                .Any(c =>
                    c.Name.ToLower() == className.ToLower() &&
                    c.EducationMedium != null &&
                    c.EducationMedium.Id == medium.Id);

            if (exists)
            {
                ModelState.AddModelError("", "This Class Already Exists For The Selected Education Medium.");
                return View(model);
            }

            var cls = new Class
            {
                Name = className,
                EducationMedium = medium,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                IsApproved = false,
                CreatedBy = currentUser
            };

            _context.Class.Add(cls);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //Edit Class
        public IActionResult Edit(long id)
        {
            var cls = _context.Class.Find(id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Class model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var existing = _context.Class.Find(id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;

            _context.Update(existing);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //Show Class Details
        public IActionResult Details(long id)
        {
            var cls = _context.Class
                .Include(c => c.EducationMedium)
                .FirstOrDefault(c => c.Id == id);
            if (cls == null) return NotFound();
            return View(cls);
        }


        //Delete Class
        public IActionResult Delete(long id)
        {
            var cls = _context.Class
                .Include(c => c.EducationMedium)
                .FirstOrDefault(c => c.Id == id);
            if (cls == null) return NotFound();
            return View(cls);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var cls = _context.Class.Find(id);
            if (cls == null) return NotFound();

            _context.Class.Remove(cls);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        //Approved Class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(long id)
        {
            var cls = _context.Class.Find(id);
            if (cls == null) return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;
            cls.IsApproved = true;
            cls.ApprovedAt = DateTime.UtcNow;
            cls.ApprovedBy = currentUser;
            cls.RejectedAt = null;
            cls.RejectedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        //Rejected Class
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(long id)
        {
            var cls = _context.Class.Find(id);
            if (cls == null) return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;
            cls.IsApproved = false;
            cls.RejectedAt = DateTime.UtcNow;
            cls.RejectedBy = currentUser;
            cls.ApprovedAt = null;
            cls.ApprovedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }

}
