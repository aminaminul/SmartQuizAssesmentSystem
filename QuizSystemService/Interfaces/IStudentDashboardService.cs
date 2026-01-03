using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IStudentDashboardService
    {
        Task<StudentDashboardViewModel> GetDashboardAsync(long studentUserId);
    }
}
