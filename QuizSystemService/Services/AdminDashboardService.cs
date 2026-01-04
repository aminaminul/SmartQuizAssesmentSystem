using QuizSystemModel.Interfaces;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _repo;

        public AdminDashboardService(IAdminDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<AdminDashboardViewModel> GetDashboardAsync()
        {
            long instructors = await _repo.GetInstructorCountAsync();
            long students = await _repo.GetStudentCountAsync();
            long quizzes = await _repo.GetQuizCountAsync();
            long pending = await _repo.GetPendingQuizCountAsync();
            long classes = await _repo.GetClassCountAsync();
            long subjects = await _repo.GetSubjectCountAsync();

            return new AdminDashboardViewModel
            {
                TotalInstructors = instructors,
                TotalStudents = students,
                TotalUsers = instructors + students,
                TotalQuizzes = quizzes,
                PendingQuizzes = pending,
                TotalClasses = classes,
                TotalSubjects = subjects
            };
        }
    }
}
