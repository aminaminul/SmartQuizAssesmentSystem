// SmartQuizAssessmentSystem/Services/AccountService.cs
using Microsoft.AspNetCore.Identity;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly RoleManager<QuizSystemRole> _roleManager;
        private readonly IAccountRepository _accountRepository;

        public AccountService(
            UserManager<QuizSystemUser> userManager,
            RoleManager<QuizSystemRole> roleManager,
            IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _accountRepository = accountRepository;
        }

        public async Task<IdentityResult> RegisterStudentAsync(StudentAddViewModel model)
        {
            const string defaultRoleName = "Student";
            var roleName = string.IsNullOrWhiteSpace(model.Role)
                ? defaultRoleName
                : model.Role!;

            // duplicate checks against Student table
            if (await _accountRepository.StudentEmailExistsAsync(model.Email!))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateStudentEmail",
                    Description = "This email is already used by another student."
                });

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _accountRepository.StudentPhoneExistsAsync(model.PhoneNumber))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateStudentPhone",
                    Description = "This phone number is already used by another student."
                });

            var ensureRoleResult = await EnsureRoleExistsAsync(roleName);
            if (!ensureRoleResult.Succeeded)
                return ensureRoleResult;

            var user = new QuizSystemUser
            {
                FirstName = model.FirstName!,
                LastName = model.LastName!,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password!);
            if (!createResult.Succeeded)
                return createResult;

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
                return roleResult;

            var student = new Student
            {
                FirstName = model.FirstName!,
                LastName = model.LastName!,
                Email = model.Email!,
                PhoneNumber = model.PhoneNumber!,
                UserId = user.Id,
                EducationMediumId = model.EducationMediumId,
                ClassId = model.ClassId,           // IMPORTANT: Class set here
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active
            };

            await _accountRepository.AddStudentAsync(student);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RegisterInstructorAsync(InstructorAddViewModel model)
        {
            const string defaultRoleName = "Instructor";
            var roleName = string.IsNullOrWhiteSpace(model.Role)
                ? defaultRoleName
                : model.Role!;

            if (await _accountRepository.InstructorEmailExistsAsync(model.Email))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateInstructorEmail",
                    Description = "This email is already used by another instructor."
                });

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber) &&
                await _accountRepository.InstructorPhoneExistsAsync(model.PhoneNumber))
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateInstructorPhone",
                    Description = "This phone number is already used by another instructor."
                });

            var ensureRoleResult = await EnsureRoleExistsAsync(roleName);
            if (!ensureRoleResult.Succeeded)
                return ensureRoleResult;

            var user = new QuizSystemUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
                return createResult;

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
                return roleResult;

            var instructor = new Instructor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                HscPassingInstrutute = model.HscPassingInstitute,
                HscPassingYear = model.HscPassingYear,
                HscGrade = model.HscGrade,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Pending,
                EducationMediumId = model.EducationMediumId
            };

            await _accountRepository.AddInstructorAsync(instructor);

            return IdentityResult.Success;
        }

        public async Task<bool> IsUserApprovedAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin")) return true;

            if (roles.Contains("Instructor"))
            {
                var instructor = await _accountRepository.GetInstructorByUserIdAsync(userId);
                return instructor != null && instructor.Status == ModelStatus.Active;
            }

            if (roles.Contains("Student"))
            {
                var student = await _accountRepository.GetStudentByUserIdAsync(userId);
                // For now students are active once registered, or we check status if we add pending status to them too.
                return student != null && student.Status == ModelStatus.Active;
            }

            return false;
        }

        private async Task<IdentityResult> EnsureRoleExistsAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Success;

            var role = new QuizSystemRole { Name = roleName };
            return await _roleManager.CreateAsync(role);
        }
    }
}
