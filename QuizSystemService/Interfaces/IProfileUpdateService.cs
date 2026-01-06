using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IProfileUpdateService
    {
        Task<InstructorProfileUpdateViewModel> GetInstructorProfileForEditAsync(long userId);
        Task<StudentProfileUpdateViewModel> GetStudentProfileForEditAsync(long userId);

        Task RequestInstructorProfileUpdateAsync(InstructorProfileUpdateViewModel model);
        Task RequestStudentProfileUpdateAsync(StudentProfileUpdateViewModel model);

        Task<List<ProfileUpdateRequest>> GetPendingRequestsAsync();
        Task ApproveProfileUpdateAsync(long requestId, long adminUserId);
        Task RejectProfileUpdateAsync(long requestId, long adminUserId, string? comment);
    }
}
