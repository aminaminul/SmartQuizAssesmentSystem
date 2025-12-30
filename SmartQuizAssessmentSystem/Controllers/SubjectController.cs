using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectController : Controller
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Subject
        public IActionResult Index()
        {
            var subjects = _context.Subject
                .Include(s => s.Class)
                .Where(s => s.Status != ModelStatus.Deleted)
                .ToList();

            return View(subjects);
        }

        // GET: Subject/Details/5
        public IActionResult Details(long id)
        {
            var subject = _context.Subject
                .Include(s => s.Class)
                .Include(s => s.CreatedBy)
                .Include(s => s.ApprovedBy)
                .Include(s => s.RejectedBy)
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
                return NotFound();

            return View(subject);
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            PopulateClassDropdown();
            return View(new Subject());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Subject model, long? classId)
        {
            if (!ModelState.IsValid)
            {
                PopulateClassDropdown(classId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Name is required.");

            if (!ModelState.IsValid)
            {
                PopulateClassDropdown(classId);
                return View(model);
            }

            var currentUser = _context.Users
                .FirstOrDefault(u => u.UserName == User.Identity!.Name);

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.CreatedBy = currentUser;
            model.IsApproved = false;

            if (classId.HasValue)
            {
                var cls = _context.Class.Find(classId.Value);
                model.Class = cls;
            }

            _context.Subject.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Edit/5
        public IActionResult Edit(long id)
        {
            var subject = _context.Subject
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
                return NotFound();

            long? classId = subject.Class?.Id;
            PopulateClassDropdown(classId);

            return View(subject);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Subject model, long? classId)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateClassDropdown(classId);
                return View(model);
            }

            var existing = _context.Subject
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (existing == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Name is required.");

            if (!ModelState.IsValid)
            {
                PopulateClassDropdown(classId);
                return View(model);
            }

            existing.Name = model.Name;
            existing.IsApproved = model.IsApproved;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;

            if (classId.HasValue)
            {
                var cls = _context.Class.Find(classId.Value);
                existing.Class = cls;
            }
            else
            {
                existing.Class = null;
            }

            _context.Update(existing);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Subject/Delete/5
        public IActionResult Delete(long id)
        {
            var subject = _context.Subject
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
                return NotFound();

            return View(subject);
        }

        // POST: Subject/Delete/5 (soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var subject = _context.Subject.Find(id);
            if (subject == null)
                return NotFound();

            subject.Status = ModelStatus.Deleted;
            subject.ModifiedAt = DateTime.UtcNow;

            _context.Subject.Update(subject);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private void PopulateClassDropdown(long? selectedId = null)
        {
            var classes = _context.Class.ToList();
            ViewBag.ClassId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                classes, "Id", "Name", selectedId);
        }
    }
}
