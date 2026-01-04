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
    }
}
