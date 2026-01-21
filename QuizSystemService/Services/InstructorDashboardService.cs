using QuizSystemModel.Interfaces;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class InstructorDashboardService : IInstructorDashboardService
    {
        private readonly IInstructorDashboardRepository _repo;

        public InstructorDashboardService(IInstructorDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<InstructorDashboardViewModel> GetDashboardAsync(long instructorUserId)
        {
            long totalStudents = await _repo.GetStudentCountAsync();
            long totalInstructors = await _repo.GetInstructorCountAsync();
            long totalMediums = await _repo.GetEducationMediumCountAsync();
            long totalClasses = await _repo.GetClassCountAsync();
            long totalSubjects = await _repo.GetSubjectCountAsync();
            long totalQuizzesForInstr = await _repo.GetQuizCountForInstructorAsync(instructorUserId);

            return new InstructorDashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalEducationMediums = totalMediums,
                TotalClasses = totalClasses,
                TotalSubjects = totalSubjects,
                TotalQuizzes = totalQuizzesForInstr,
                StudentPerformanceAvg = await _repo.GetStudentPerformanceAvgAsync(instructorUserId),
                ClassPerformanceAvg = await _repo.GetClassPerformanceAvgAsync(instructorUserId),
                EducationMediumPerformanceAvg = await _repo.GetEducationMediumPerformanceAvgAsync(instructorUserId)
            };
        }
    }
}
