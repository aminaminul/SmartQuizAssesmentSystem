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

        public Task<List<Student>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Student?> GetByIdAsync(long id, bool includeUser = false) =>
            _repo.GetByIdAsync(id, includeUser);

        public Task<List<EducationMedium>> GetEducationMediumsAsync() =>
            _repo.GetEducationMediumsAsync();

        public Task<List<Class>> GetClassesAsync() =>
            _repo.GetClassesAsync();

        public async Task<bool> CreateAsync(StudentAddView model, QuizSystemUser currentUser)
        {
            if (await _repo.EmailExistsAsync(model.Email!))
                throw new InvalidOperationException("This Email Is Already Used By Another Student.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber))
                throw new InvalidOperationException("This Phone Number Is Already Used By Another Student.");

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
                throw new InvalidOperationException(string.Join(" | ", userResult.Errors.Select(e => e.Description)));

            var roleName = model.Role ?? "Student";
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new QuizSystemRole { Name = roleName });

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", roleResult.Errors.Select(e => e.Description)));

            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EducationMediumId = model.EducationMediumId,
                ClassId = model.ClassId,      // Class save
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                CreatedBy = currentUser
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
                throw new InvalidOperationException("This email is already used by another student.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber, id))
                throw new InvalidOperationException("This phone number is already used by another student.");

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Status = model.Status;
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
