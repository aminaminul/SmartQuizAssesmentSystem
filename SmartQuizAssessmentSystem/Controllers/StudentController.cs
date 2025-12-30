using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemRepository.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;

        public StudentController(
            AppDbContext context,
            UserManager<QuizSystemUser> userManager,
            RoleManager<QuizSystemRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var students = _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .Where(s => s.Status != ModelStatus.Deleted)
                .ToList();

            return View(students);
        }

        public IActionResult Details(long id)
        {
            var student = _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .Include(s => s.User)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // Create Student
        public IActionResult Create()
        {
            PopulateDropdowns();
            var vm = new StudentAddView { Role = "Student" };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentAddView model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.EducationMediumId);
                return View(model);
            }

            //Checks Duplicate Email and Phone Number
            bool emailExists = _context.Student
                .Any(s => s.Email != null && s.Email.ToLower() == model.Email!.ToLower());
            if (emailExists)
                ModelState.AddModelError("Email", "This Email Is Already Used By Another Student.");

            bool phoneExists = _context.Student
                .Any(s => s.PhoneNumber != null && s.PhoneNumber == model.PhoneNumber);
            if (phoneExists)
                ModelState.AddModelError("PhoneNumber", "This Phone Number Is Already Used By Another Student.");

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model.EducationMediumId);
                return View(model);
            }

            //Create Identity user
            var user = new QuizSystemUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var userResult = await _userManager.CreateAsync(user, model.Password!);
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                PopulateDropdowns(model.EducationMediumId);
                return View(model);
            }

            //Ensure Roles
            if (!await _roleManager.RoleExistsAsync(model.Role!))
            {
                await _roleManager.CreateAsync(new QuizSystemRole { Name = model.Role });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role!);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                PopulateDropdowns(model.EducationMediumId);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            //Create Student Entity
            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EducationMediumId = model.EducationMediumId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                CreatedBy = currentUser
            };

            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Update Student
        public IActionResult Edit(long id)
        {
            var student = _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            PopulateDropdowns(student.EducationMediumId, student.ClassId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Student model, long? educationMediumId, long? classId)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(educationMediumId, classId);
                return View(model);
            }

            var existing = _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (existing == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                bool emailExists = _context.Student
                    .Any(s => s.Id != id &&
                              s.Email != null &&
                              s.Email.ToLower() == model.Email.ToLower());
                if (emailExists)
                    ModelState.AddModelError("Email", "This email is already used by another student.");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                bool phoneExists = _context.Student
                    .Any(s => s.Id != id &&
                              s.PhoneNumber != null &&
                              s.PhoneNumber == model.PhoneNumber);
                if (phoneExists)
                    ModelState.AddModelError("PhoneNumber", "This phone number is already used by another student.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(educationMediumId, classId);
                return View(model);
            }

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.EducationMediumId = educationMediumId;
            existing.ClassId = classId;

            _context.Update(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete Student
        public IActionResult Delete(long id)
        {
            var student = _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var student = _context.Student.Find(id);
            if (student == null)
                return NotFound();

            student.Status = ModelStatus.Deleted;
            student.ModifiedAt = DateTime.UtcNow;

            _context.Student.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(long? educationMediumId = null, long? classId = null)
        {
            ViewBag.EducationMediumId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _context.EducationMedium.ToList(), "Id", "Name", educationMediumId);
        }
    }
}
