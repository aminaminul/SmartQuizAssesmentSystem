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
            long mediums = await _repo.GetEducationMediumCountAsync();
            long pendingClasses = await _repo.GetPendingClassCountAsync();
            long pendingSubjects = await _repo.GetPendingSubjectCountAsync();

            return new AdminDashboardViewModel
            {
                TotalInstructors = instructors,
                TotalStudents = students,
                TotalUsers = instructors + students,
                TotalQuizzes = quizzes,
                TotalClasses = classes,
                TotalSubjects = subjects,
                TotalEducationMediums = mediums,
                PendingClasses = pendingClasses,
                PendingSubjects = pendingSubjects,
                PendingQuizzes = pending,
                PendingInstructors = await _repo.GetPendingInstructorCountAsync(),
                PendingEducationMediums = await _repo.GetPendingEducationMediumCountAsync(),
                PendingProfileUpdates = await _repo.GetPendingProfileUpdateCountAsync()
            };
        }

        public async Task<GlobalSearchViewModel> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new GlobalSearchViewModel();

            return new GlobalSearchViewModel
            {
                Query = query,
                Instructors = await _repo.SearchInstructorsAsync(query),
                Students = await _repo.SearchStudentsAsync(query),
                Quizzes = await _repo.SearchQuizzesAsync(query),
                Subjects = await _repo.SearchSubjectsAsync(query)
            };
        }
    }
}
