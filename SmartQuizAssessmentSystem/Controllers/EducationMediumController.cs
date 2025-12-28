using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.ViewModels;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EducationMediumController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;

        public EducationMediumController(AppDbContext context, UserManager<QuizSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //EducationMedium Index
        public IActionResult Index(long? selectedMediumId)
        {
            var mediums = _context.EducationMedium.ToList();
            ViewBag.Mediums = mediums;
            ViewBag.SelectedMediumId = selectedMediumId;

            var classes = Enumerable.Empty<Class>().ToList();

            if (selectedMediumId.HasValue)
            {
                classes = _context.Class
                    .Include(c => c.EducationMedium)
                    .Where(c => c.EducationMedium != null &&
                                c.EducationMedium.Id == selectedMediumId.Value)
                    .ToList();
            }

            ViewBag.Classes = classes;

            return View();
        }

        //EducationMedium Create
        public IActionResult Create()
        {
            var vm = new ClassMediumView();
            return View(vm);
        }

        //EducationMedium Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClassMediumView model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var mediumName = model.Medium.ToString();

            // Prevent Duplicate Mediums
            bool exists = _context.EducationMedium
                .Any(m => m.Name.ToLower() == mediumName.ToLower());

            if (exists)
            {
                ModelState.AddModelError("", "This Education Medium Already Exists.");
                return View(model);
            }

            var currentUser = _userManager.GetUserAsync(User).Result;

            var entity = new EducationMedium
            {
                Name = mediumName,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                IsApproved = false,
                CreatedBy = currentUser
            };

            _context.EducationMedium.Add(entity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //Edit Education Medium
        public IActionResult Edit(long id)
        {
            var medium = _context.EducationMedium.Find(id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, EducationMedium model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var existing = _context.EducationMedium.Find(id);
            if (existing == null)
                return NotFound();

            existing.Name = model.Name;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.Status = model.Status;

            _context.Update(existing);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //Delete Education Medium
        public IActionResult Delete(long id)
        {
            var medium = _context.EducationMedium
                .FirstOrDefault(m => m.Id == id);
            if (medium == null)
                return NotFound();

            return View(medium);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var medium = _context.EducationMedium.Find(id);
            if (medium == null)
                return NotFound();

            var relatedClasses = _context.Class
                .Include(c => c.EducationMedium)
                .Where(c => c.EducationMedium != null && c.EducationMedium.Id == id)
                .ToList();

            _context.Class.RemoveRange(relatedClasses);

            _context.EducationMedium.Remove(medium);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //Approve Education Medium
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(long id)
        {
            var medium = _context.EducationMedium.Find(id);
            if (medium == null)
                return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;

            medium.IsApproved = true;
            medium.ApprovedAt = DateTime.UtcNow;
            medium.ApprovedBy = currentUser;
            medium.RejectedAt = null;
            medium.RejectedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        //Reject Education Medium
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(long id)
        {
            var medium = _context.EducationMedium.Find(id);
            if (medium == null)
                return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;

            medium.IsApproved = false;
            medium.RejectedAt = DateTime.UtcNow;
            medium.RejectedBy = currentUser;
            medium.ApprovedAt = null;
            medium.ApprovedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
