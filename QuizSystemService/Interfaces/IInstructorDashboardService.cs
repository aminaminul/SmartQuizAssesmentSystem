using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IInstructorDashboardService
    {
        Task<InstructorDashboardViewModel> GetDashboardAsync(long instructorUserId);
    }
}
