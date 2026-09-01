using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IPerformanceService
    {
        Task<StudentPerformanceViewModel> GetStudentPerformanceAsync(long? studentId = null, long? classId = null, long? mediumId = null);
        Task<ClassPerformanceViewModel> GetClassPerformanceAsync(long? classId = null, long? mediumId = null);
        Task<MediumPerformanceViewModel> GetMediumPerformanceAsync(long? mediumId = null);
        Task<MyPerformanceViewModel> GetMyPerformanceAsync(long studentUserId);
    }
}
