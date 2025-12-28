using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;

        public SubjectController(AppDbContext context, UserManager<QuizSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var subjects = _context.Subject
                .Include(s => s.Class)
                .ToList();
            return View(subjects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Subject subject)
        {
            if (!ModelState.IsValid)
                return View(subject);

            var currentUser = _userManager.GetUserAsync(User).Result;

            subject.CreatedAt = DateTime.UtcNow;
            subject.Status = ModelStatus.Active;
            subject.IsApproved = false;
            subject.CreatedBy = currentUser;

            _context.Subject.Add(subject);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(long id)
        {
            var subject = _context.Subject.Find(id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Subject subject)
        {
            if (id != subject.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(subject);

            var existing = _context.Subject
                .Include(s => s.CreatedBy)
                .FirstOrDefault(s => s.Id == id);

            if (existing == null)
                return NotFound();

            existing.Name = subject.Name;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.Status = subject.Status;

            _context.Update(existing);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(long id)
        {
            var subject = _context.Subject
                .FirstOrDefault(m => m.Id == id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var subject = _context.Subject.Find(id);
            if (subject == null)
                return NotFound();

            _context.Subject.Remove(subject);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(long id)
        {
            var subject = _context.Subject.Find(id);
            if (subject == null)
                return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;

            subject.IsApproved = true;
            subject.ApprovedAt = DateTime.UtcNow;
            subject.ApprovedBy = currentUser;
            subject.RejectedAt = null;
            subject.RejectedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(long id)
        {
            var subject = _context.Subject.Find(id);
            if (subject == null)
                return NotFound();

            var currentUser = _userManager.GetUserAsync(User).Result;

            subject.IsApproved = false;
            subject.RejectedAt = DateTime.UtcNow;
            subject.RejectedBy = currentUser;
            subject.ApprovedAt = null;
            subject.ApprovedBy = null;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}