using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using System.Linq;

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
            ViewBag.EducationMediumId = new SelectList(mediums, "Id", "Name", educationMediumId);

            return View(classes.ToList());
        }

        public IActionResult Create()
        {
            var mediums = _context.EducationMedium.ToList();
            ViewBag.EducationMediumId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                mediums, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Class model, long? educationMediumId)
        {
            if (!ModelState.IsValid)
            {
                var mediums = _context.EducationMedium.ToList();
                ViewBag.EducationMediumId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    mediums, "Id", "Name", educationMediumId);
                return View(model);
            }

            long? mediumId = educationMediumId;
            if (model.EducationMedium != null && model.EducationMedium.Id != 0)
                mediumId = model.EducationMedium.Id;

            bool exists = _context.Class
                .Include(c => c.EducationMedium)
                .Any(c =>
                    c.Name.ToLower() == model.Name.ToLower() &&
                    c.EducationMedium != null &&
                    c.EducationMedium.Id == mediumId);

            if (exists)
            {
                ModelState.AddModelError("Name", "This class already exists for the selected education medium.");
                var mediums = _context.EducationMedium.ToList();
                ViewBag.EducationMediumId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                    mediums, "Id", "Name", educationMediumId);
                return View(model);
            }

            var currentUser = _userManager.GetUserAsync(User).Result;

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;

            if (mediumId.HasValue)
            {
                var medium = _context.EducationMedium.Find(mediumId.Value);
                model.EducationMedium = medium;
            }

            _context.Class.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


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

        public IActionResult Details(long id)
        {
            var cls = _context.Class
                .Include(c => c.EducationMedium)
                .FirstOrDefault(c => c.Id == id);
            if (cls == null) return NotFound();
            return View(cls);
        }

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
