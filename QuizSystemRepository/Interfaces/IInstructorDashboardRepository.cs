using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IInstructorDashboardRepository
    {
        Task<long> GetStudentCountAsync();
        Task<long> GetInstructorCountAsync();
        Task<long> GetEducationMediumCountAsync();
        Task<long> GetClassCountAsync();
        Task<long> GetSubjectCountAsync();
        Task<long> GetQuizCountForInstructorAsync(long instructorUserId);
        Task<double> GetStudentPerformanceAvgAsync(long instructorUserId);
        Task<double> GetClassPerformanceAvgAsync(long instructorUserId);
        Task<double> GetEducationMediumPerformanceAvgAsync(long instructorUserId);
    }
}
