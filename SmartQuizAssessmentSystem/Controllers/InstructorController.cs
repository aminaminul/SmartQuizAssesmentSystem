using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemRepository.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;

        public InstructorController(AppDbContext context,RoleManager<QuizSystemRole> rolemanager, UserManager<QuizSystemUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = rolemanager;
        }

        //Instructor
        public IActionResult Index()
        {
            var instructors = _context.Instructor
                .Include(i => i.EducationMedium)
                .Where(i => i.Status != ModelStatus.Deleted)
                .ToList();

            return View(instructors);
        }


        //Instructor Create
        public IActionResult Create()
        {
            PopulateEducationMediumDropdown();
            var vm = new RegisterViewModel();
            return View(vm);
        }

        //Instructor Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(model.EducationMediumId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email Is Required.");
            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                ModelState.AddModelError("PhoneNumber", "Phone Number Is Required.");
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("Password", "Password Is Required.");

            bool emailExists = _context.Instructor
                .Any(i => i.Email != null &&
                          i.Email.ToLower() == model.Email.ToLower());
            if (emailExists)
                ModelState.AddModelError("Email", "This Email Is Already Used By Another Instructor.");

            bool phoneExists = _context.Instructor
                .Any(i => i.PhoneNumber != null &&
                          i.PhoneNumber == model.PhoneNumber);
            if (phoneExists)
                ModelState.AddModelError("PhoneNumber", "This Phone Number Is Already Used By Another Instructor.");

            // stop here if any validation error
            if (!ModelState.IsValid)
            {
                PopulateEducationMediumDropdown(model.EducationMediumId);
                return View(model);
            }

            // 1) Create Identity user
            var user = new QuizSystemUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            IdentityResult userResult = await _userManager.CreateAsync(user, model.Password);
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                PopulateEducationMediumDropdown(model.EducationMediumId);
                return View(model);
            }

            // 2) Ensure role exists and add to role
            if (!await _roleManager.RoleExistsAsync("Instructor"))
            {
                await _roleManager.CreateAsync(new QuizSystemRole { Name = "Instructor" });
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Instructor");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                PopulateEducationMediumDropdown(model.EducationMediumId);
                return View(model);
            }

            // 3) Create Instructor
            var currentUser = await _userManager.GetUserAsync(User);

            var instructor = new Instructor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                HscPassingInstrutute = model.HscPassingInstitute,
                HscPassingYear = model.HscPassingYear,
                HscGrade = model.HscGrade,
                EducationMediumId = model.EducationMediumId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                CreatedBy = currentUser
            };

            _context.Instructor.Add(instructor);
            await _context.SaveChangesAsync();

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

            instructor.Status = ModelStatus.Deleted;
            instructor.ModifiedAt = DateTime.UtcNow;

            _context.Instructor.Update(instructor);
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
