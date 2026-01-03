using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IStudentDashboardRepository
    {
        Task<List<Quiz>> GetAvailableQuizzesAsync(long studentUserId, DateTime now);
        Task<List<QuizAttempt>> GetStudentAttemptsAsync(long studentUserId);
    }
}
