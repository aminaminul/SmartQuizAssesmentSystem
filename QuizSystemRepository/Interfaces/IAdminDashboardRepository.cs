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
    }

}
