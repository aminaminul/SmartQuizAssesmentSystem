using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class ProfileUpdateService : IProfileUpdateService
    {
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly IProfileUpdateRepository _profileRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IStudentRepository _studentRepo;

        private readonly TimeSpan _cooldown = TimeSpan.FromDays(90);

        public ProfileUpdateService(
            UserManager<QuizSystemUser> userManager,
            IProfileUpdateRepository profileRepo,
            IInstructorRepository instructorRepo,
            IStudentRepository studentRepo)
        {
            _userManager = userManager;
            _profileRepo = profileRepo;
            _instructorRepo = instructorRepo;
            _studentRepo = studentRepo;
        }

        

        public async Task<InstructorProfileUpdateViewModel> GetInstructorProfileForEditAsync(long userId)
        {
            var instructor = await _instructorRepo.GetByUserIdAsync(userId);
            if (instructor == null)
                throw new InvalidOperationException("Instructor profile not found.");

            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return new InstructorProfileUpdateViewModel
            {
                InstructorId = instructor.Id,
                UserId = userId,
                FirstName = instructor.FirstName ?? user.FirstName,
                LastName = instructor.LastName ?? user.LastName,
                Email = instructor.Email ?? user.Email!,
                PhoneNumber = instructor.PhoneNumber ?? user.PhoneNumber!,
                HscPassingInstitute = instructor.HscPassingInstrutute ?? "",
                HscPassingYear = instructor.HscPassingYear,
                HscGrade = instructor.HscGrade ?? "",
                EducationMediumId = instructor.EducationMediumId,
                EducationMediumName = instructor.EducationMedium?.Name,
                ClassId = instructor.ClassId,
                ClassName = instructor.Class?.Name
            };
        }

        public async Task<StudentProfileUpdateViewModel> GetStudentProfileForEditAsync(long userId)
        {
            var student = await _studentRepo.GetByUserIdAsync(userId);
            if (student == null)
                throw new InvalidOperationException("Student profile not found.");

            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return new StudentProfileUpdateViewModel
            {
                StudentId = student.Id,
                UserId = userId,
                FirstName = student.FirstName ?? user.FirstName,
                LastName = student.LastName ?? user.LastName,
                Email = student.Email ?? user.Email!,
                PhoneNumber = student.PhoneNumber ?? user.PhoneNumber!,
                EducationMediumId = student.EducationMediumId,
                EducationMediumName = student.EducationMedium?.Name,
                ClassId = student.ClassId,
                ClassName = student.Class?.Name
            };
        }

        

        public async Task RequestInstructorProfileUpdateAsync(InstructorProfileUpdateViewModel model)
        {
            await EnsureCanRequestAsync(model.UserId);

            var request = new ProfileUpdateRequest
            {
                UserId = model.UserId,
                Role = "Instructor",
                OldDataJson = "",
                NewDataJson = JsonSerializer.Serialize(model),
                Status = ProfileUpdateStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _profileRepo.AddAsync(request);
        }

        public async Task RequestStudentProfileUpdateAsync(StudentProfileUpdateViewModel model)
        {
            await EnsureCanRequestAsync(model.UserId);

            var request = new ProfileUpdateRequest
            {
                UserId = model.UserId,
                Role = "Student",
                OldDataJson = "",
                NewDataJson = JsonSerializer.Serialize(model),
                Status = ProfileUpdateStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _profileRepo.AddAsync(request);
        }

        private async Task EnsureCanRequestAsync(long userId)
        {
            var lastApproved = await _profileRepo.GetLastApprovedAsync(userId);
            if (lastApproved?.ApprovedAt != null)
            {
                var next = lastApproved.ApprovedAt.Value + _cooldown;
                if (DateTime.UtcNow < next)
                    throw new InvalidOperationException("Profile can only be updated once every 3 months.");
            }

            var pending = await _profileRepo.GetLastPendingAsync(userId);
            if (pending != null)
                throw new InvalidOperationException("You already have a pending profile update request.");
        }

        

        public Task<List<ProfileUpdateRequest>> GetPendingRequestsAsync()
        {
            return _profileRepo.GetPendingRequestsAsync();
        }

        public async Task ApproveProfileUpdateAsync(long requestId, long adminUserId)
        {
            var request = await _profileRepo.GetByIdAsync(requestId);
            if (request == null || request.Status != ProfileUpdateStatus.Pending)
                throw new InvalidOperationException("Request not found or already processed.");

            if (request.Role == "Instructor")
            {
                var vm = JsonSerializer.Deserialize<InstructorProfileUpdateViewModel>(request.NewDataJson)
                         ?? throw new InvalidOperationException("Invalid instructor data.");
                await ApplyInstructorUpdateAsync(vm);
            }
            else if (request.Role == "Student")
            {
                var vm = JsonSerializer.Deserialize<StudentProfileUpdateViewModel>(request.NewDataJson)
                         ?? throw new InvalidOperationException("Invalid student data.");
                await ApplyStudentUpdateAsync(vm);
            }

            request.Status = ProfileUpdateStatus.Approved;
            request.ApprovedAt = DateTime.UtcNow;
            request.LastAppliedAt = request.ApprovedAt;
            await _profileRepo.UpdateAsync(request);
        }

        public async Task RejectProfileUpdateAsync(long requestId, long adminUserId, string? comment)
        {
            var request = await _profileRepo.GetByIdAsync(requestId);
            if (request == null || request.Status != ProfileUpdateStatus.Pending)
                throw new InvalidOperationException("Request not found or already processed.");

            request.Status = ProfileUpdateStatus.Rejected;
            request.RejectedAt = DateTime.UtcNow;
            request.AdminComment = comment;
            await _profileRepo.UpdateAsync(request);
        }

        

        private async Task ApplyInstructorUpdateAsync(InstructorProfileUpdateViewModel vm)
        {
            var instructor = await _instructorRepo.GetByIdAsync(vm.InstructorId);
            if (instructor == null) throw new InvalidOperationException("Instructor not found.");

            var user = await _userManager.Users.FirstAsync(u => u.Id == vm.UserId);

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.NormalizedEmail = vm.Email.ToUpperInvariant();
            user.NormalizedUserName = vm.Email.ToUpperInvariant();
            user.PhoneNumber = vm.PhoneNumber;

            instructor.FirstName = vm.FirstName;
            instructor.LastName = vm.LastName;
            instructor.Email = vm.Email;
            instructor.PhoneNumber = vm.PhoneNumber;
            instructor.HscPassingInstrutute = vm.HscPassingInstitute;
            instructor.HscPassingYear = vm.HscPassingYear;
            instructor.HscGrade = vm.HscGrade;
            instructor.EducationMediumId = vm.EducationMediumId;
            instructor.ClassId = vm.ClassId;
            instructor.ModifiedAt = DateTime.UtcNow;

            await _instructorRepo.UpdateAsync(instructor);
            await _userManager.UpdateAsync(user);
        }

        private async Task ApplyStudentUpdateAsync(StudentProfileUpdateViewModel vm)
        {
            var student = await _studentRepo.GetByIdAsync(vm.StudentId);
            if (student == null) throw new InvalidOperationException("Student not found.");

            var user = await _userManager.Users.FirstAsync(u => u.Id == vm.UserId);

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.NormalizedEmail = vm.Email.ToUpperInvariant();
            user.NormalizedUserName = vm.Email.ToUpperInvariant();
            user.PhoneNumber = vm.PhoneNumber;

            student.FirstName = vm.FirstName;
            student.LastName = vm.LastName;
            student.Email = vm.Email;
            student.PhoneNumber = vm.PhoneNumber;
            student.EducationMediumId = vm.EducationMediumId;
            student.ClassId = vm.ClassId;
            student.ModifiedAt = DateTime.UtcNow;

            await _studentRepo.UpdateAsync(student);
            await _userManager.UpdateAsync(user);
        }
    }
}
