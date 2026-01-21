using QuizSystemModel.Models;

namespace QuizSystemModel.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; }
        public int TotalQuizzesAvailable { get; set; }
        public int TotalQuizzesCompleted { get; set; }
        public decimal AverageScore { get; set; }
        public double AveragePerformancePercentage { get; set; }

        public List<Quiz> AvailableQuizzes { get; set; } = new();
        public List<QuizAttempt> RecentAttempts { get; set; } = new();
    }
}
