using Microsoft.AspNetCore.Identity;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;

        public StudentService(
            IStudentRepository repo,
            UserManager<QuizSystemUser> userManager,
            RoleManager<QuizSystemRole> roleManager)
        {
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(long id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Student?> GetForEditAsync(long id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<EducationMedium>> GetEducationMediumsAsync()
        {
            return await _repo.GetEducationMediumsAsync();
        }

        public async Task<List<Class>> GetClassesAsync()
        {
            return await _repo.GetClassesAsync();
        }

        public async Task<bool> CreateAsync(StudentAddViewModel model, QuizSystemUser currentUser)
        {
            // Email and Phone Verify
            if (await _repo.EmailExistsAsync(model.Email))
                throw new InvalidOperationException("This Email Is Already Used By Another Student.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber))
                throw new InvalidOperationException("This Phone Number Is Already Used By Another Student.");

            // Identity User
            var user = new QuizSystemUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var userResult = await _userManager.CreateAsync(user, model.Password);
            if (!userResult.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", userResult.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", roleResult.Errors.Select(e => e.Description)));

            // Student
            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EducationMediumId = model.EducationMediumId,
                ClassId = model.ClassId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                ModifiedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(student);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Student model, long? educationMediumId, long? classId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (!string.IsNullOrWhiteSpace(model.Email) &&
                await _repo.EmailExistsAsync(model.Email, id))
                throw new InvalidOperationException("This Email Is Already Used By Another Student.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber, id))
                throw new InvalidOperationException("This Phone Number Is Already Used By Another Student.");

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Status = model.Status;
            existing.ModifiedBy = model.User;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.EducationMediumId = educationMediumId;
            existing.ClassId = classId;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student == null)
                return false;

            student.Status = ModelStatus.Deleted;
            student.ModifiedAt = DateTime.UtcNow;
            student.ModifiedBy = currentUser;

            await _repo.UpdateAsync(student);
            return true;
        }
    }
}
