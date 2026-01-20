using Microsoft.AspNetCore.Identity;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IInstructorRepository _repo;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;

        public InstructorService(
            IInstructorRepository repo,
            UserManager<QuizSystemUser> userManager,
            RoleManager<QuizSystemRole> roleManager)
        {
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<Instructor>> GetAllAsync() 
        { 
            return await _repo.GetAllAsync(); 
        } 

        public async Task<Instructor?> GetByIdAsync(long id)
        { 
            return await _repo.GetByIdAsync(id); 
        }

        public async Task<Instructor?> GetForEditAsync(long id)
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

        public async Task<Instructor?> GetByUserIdAsync(long userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<bool> CreateAsync(InstructorAddViewModel model, QuizSystemUser currentUser)
        {
            // Email and Phone Verify
            if (await _repo.EmailExistsAsync(model.Email))
                throw new InvalidOperationException("This Email Is Already Used By Another Instructor.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber))
                throw new InvalidOperationException("This Phone Number Is Already Used By Another Instructor.");

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

            var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(" | ", roleResult.Errors.Select(e => e.Description)));

            // Instructor
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
                ClassId = model.ClassId,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Pending,
                CreatedBy = currentUser,
                ModifiedBy= currentUser,
                ModifiedAt= DateTime.UtcNow
                
            };

            await _repo.AddAsync(instructor);
            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var instructor = await _repo.GetByIdAsync(id);
            if (instructor == null) return false;

            instructor.Status = ModelStatus.Active;
            instructor.ModifiedBy = currentUser;
            instructor.ModifiedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(instructor);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var instructor = await _repo.GetByIdAsync(id);
            if (instructor == null) return false;

            instructor.Status = ModelStatus.InActive;
            instructor.ModifiedBy = currentUser;
            instructor.ModifiedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(instructor);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Instructor model, long? educationMediumId, long? classId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (!string.IsNullOrWhiteSpace(model.Email) &&
                await _repo.EmailExistsAsync(model.Email, id))
                throw new InvalidOperationException("This Email Is Already Used By Another Instructor.");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _repo.PhoneExistsAsync(model.PhoneNumber, id))
                throw new InvalidOperationException("This Phone Number Is Already Used By Another Instructor.");

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.HscPassingInstrutute = model.HscPassingInstrutute;
            existing.HscPassingYear = model.HscPassingYear;
            existing.HscGrade = model.HscGrade;
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
            var instructor = await _repo.GetByIdAsync(id);
            if (instructor == null)
                return false;

            instructor.Status = ModelStatus.Deleted;
            instructor.ModifiedAt = DateTime.UtcNow;
            instructor.ModifiedBy = currentUser;

            await _repo.UpdateAsync(instructor);
            return true;
        }
    }
}
