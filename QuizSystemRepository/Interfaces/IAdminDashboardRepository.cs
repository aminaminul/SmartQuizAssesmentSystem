using System.Collections.Generic;
using System.Threading.Tasks;
using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<long> GetInstructorCountAsync();
        Task<long> GetStudentCountAsync();
        Task<long> GetQuizCountAsync();
        Task<long> GetPendingQuizCountAsync();
        Task<long> GetClassCountAsync();
        Task<long> GetSubjectCountAsync();
        Task<long> GetEducationMediumCountAsync();
        Task<long> GetPendingClassCountAsync();
        Task<long> GetPendingSubjectCountAsync();
        Task<long> GetPendingInstructorCountAsync();
        Task<long> GetPendingEducationMediumCountAsync();
        Task<long> GetPendingProfileUpdateCountAsync();

        Task<List<Instructor>> SearchInstructorsAsync(string query);
        Task<List<Student>> SearchStudentsAsync(string query);
        Task<List<Quiz>> SearchQuizzesAsync(string query);
        Task<List<Subject>> SearchSubjectsAsync(string query);

        Task<double> GetStudentPerformanceAvgAsync();
        Task<double> GetClassPerformanceAvgAsync();
        Task<double> GetEducationMediumPerformanceAvgAsync();
        Task<List<QuizAttempt>> GetTopPerformingStudentsAsync(int count);
    }
}
