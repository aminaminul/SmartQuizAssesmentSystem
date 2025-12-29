using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using System.Linq;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;

        public InstructorController(AppDbContext context, UserManager<QuizSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Instructor
        public IActionResult Index()
        {
            var instructors = _context.Instructor
                .Include(i => i.EducationMedium)
                .ToList();
            return View(instructors);
        }

        //Instructor Create
        public IActionResult Create()
        {
            PopulateEducationMediumDropdown();
            return View();
        }

        //Instructor Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Instructor model, long? educationMediumId)
        {
            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(educationMediumId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email Is Required.");
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Phone Number Is Required.");

            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(educationMediumId);
                return View(model);
            }

            //Duplicate Email
            bool emailExists = _context.Instructor
                .Any(i => i.Email != null &&
                          i.Email.ToLower() == model.Email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError("Email", "This Email Is Already Used By Another Instructor.");
            }

            //Duplicate Phone
            bool phoneExists = _context.Instructor
                .Any(i => i.PhoneNumber != null &&
                          i.PhoneNumber == model.PhoneNumber);

            if (phoneExists)
            {
                ModelState.AddModelError("PhoneNumber", "This Phone Number Is Already Used By Another Instructor.");
            }

            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(educationMediumId);
                return View(model);
            }

            var currentUser = _userManager.GetUserAsync(User).Result;

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.CreatedBy = currentUser;

            if (educationMediumId.HasValue)
            {
                var medium = _context.EducationMedium.Find(educationMediumId.Value);
                model.EducationMedium = medium;
            }

            _context.Instructor.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //Instructor Edit
        public IActionResult Edit(long id)
        {
            var instructor = _context.Instructor
                .Include(i => i.EducationMedium)
                .FirstOrDefault(i => i.Id == id);
            if (instructor == null)
                return NotFound();

            long? selectedMediumId = instructor.EducationMedium?.Id;
            PopulateEducationMediumDropdown(selectedMediumId);

            return View(instructor);
        }

        //Instructor Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, Instructor model, long? educationMediumId)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(educationMediumId);
                return View(model);
            }

            var existing = _context.Instructor
                .Include(i => i.EducationMedium)
                .FirstOrDefault(i => i.Id == id);
            if (existing == null)
                return NotFound();

            // Checks Email and Phone Number Duplicacy
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                bool emailExists = _context.Instructor
                    .Any(i => i.Id != id &&
                              i.Email != null &&
                              i.Email.ToLower() == model.Email.ToLower());
                if (emailExists)
                    ModelState.AddModelError("Email", "This Email Is Already Used By Another Instructor.");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                bool phoneExists = _context.Instructor
                    .Any(i => i.Id != id &&
                              i.PhoneNumber != null &&
                              i.PhoneNumber == model.PhoneNumber);
                if (phoneExists)
                    ModelState.AddModelError("PhoneNumber", "This Phone Number Is Already Used By Another Instructor.");
            }

            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(educationMediumId);
                return View(model);
            }

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.HscPassingInstrutute = model.HscPassingInstrutute;
            existing.HscPassingYear = model.HscPassingYear;
            existing.HscGrade = model.HscGrade;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;

            if (educationMediumId.HasValue)
            {
                var medium = _context.EducationMedium.Find(educationMediumId.Value);
                existing.EducationMedium = medium;
            }
            else
            {
                existing.EducationMedium = null;
            }

            _context.Update(existing);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //Instructor Details
        public IActionResult Details(long id)
        {
            var instructor = _context.Instructor
                .Include(i => i.EducationMedium)
                .Include(i => i.User)
                .FirstOrDefault(i => i.Id == id);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        //Instructor Delete
        public IActionResult Delete(long id)
        {
            var instructor = _context.Instructor
                .Include(i => i.EducationMedium)
                .FirstOrDefault(i => i.Id == id);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }

        //Instructor Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(long id)
        {
            var instructor = _context.Instructor.Find(id);
            if (instructor == null)
                return NotFound();

            _context.Instructor.Remove(instructor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private void PopulateEducationMediumDropdown(long? selectedId = null)
        {
            var mediums = _context.EducationMedium.ToList();
            ViewBag.EducationMediumId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                mediums, "Id", "Name", selectedId);
        }
    }
}
