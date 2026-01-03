using QuizSystemModel.Interfaces;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IStudentDashboardRepository _dashboardRepository;

        public StudentDashboardService(IStudentDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<StudentDashboardViewModel> GetDashboardAsync(long studentUserId)
        {
            var now = DateTime.UtcNow;

            var availableQuizzes = await _dashboardRepository.GetAvailableQuizzesAsync(studentUserId, now);
            var attempts = await _dashboardRepository.GetStudentAttemptsAsync(studentUserId);

            var completedAttempts = attempts.Where(a => a.IsSubmitted).ToList();

            var model = new StudentDashboardViewModel
            {
                TotalQuizzesAvailable = availableQuizzes.Count,
                TotalQuizzesCompleted = completedAttempts.Count,
                AverageScore = completedAttempts.Count > 0
                    ? completedAttempts.Average(a => a.TotalScore)
                    : 0,
                AvailableQuizzes = availableQuizzes,
                RecentAttempts = attempts.Take(5).ToList()
            };

            return model;
        }
    }
}
