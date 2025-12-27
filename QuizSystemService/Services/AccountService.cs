using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemRepository.Data;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountService(
            UserManager<QuizSystemUser> userManager,
            RoleManager<QuizSystemRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            //Ensure role exists
            if (string.IsNullOrWhiteSpace(model.Role))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Role is required."
                });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                var role = new QuizSystemRole { Name = model.Role };
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    return roleResult;
                }
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            //Assign role
            var roleAssignResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleAssignResult.Succeeded)
            {
                return roleAssignResult;
            }

            if (model.Role == "Student")
            {
                var student = new Student
                {
                    Name = $"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active
                };
                _context.Student.Add(student);
            }
            else if (model.Role == "Instructor")
            {
                var instructor = new Instructor
                {
                    Name = $"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    HscPassingInstrutute = model.HscPassingInstitute,
                    HscPassingYear = model.HscPassingYear,
                    HscGrade = model.HscGrade,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    Status = ModelStatus.Active,
                    EducationMedium = model.EducationMediumId
                };
                _context.Instructor.Add(instructor);
            }

            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }
}
