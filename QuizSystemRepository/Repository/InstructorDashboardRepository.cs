using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Interfaces;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class InstructorDashboardRepository : IInstructorDashboardRepository
    {
        private readonly AppDbContext _context;

        public InstructorDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> GetStudentCountAsync()
        {
            var query = _context.Student.AsQueryable();
            var total = await query.LongCountAsync();
            return total;
        }

        public async Task<long> GetInstructorCountAsync()
        {
            var query = _context.Instructor.AsQueryable();
            var total = await query.LongCountAsync();
            return total;
        }

        public async Task<long> GetEducationMediumCountAsync()
        {
            var query = _context.EducationMedium.AsQueryable();
            var total = await query.LongCountAsync();
            return total;
        }

        public async Task<long> GetClassCountAsync()
        {
            var query = _context.Class.AsQueryable();
            var total = await query.LongCountAsync();
            return total;
        }

        public async Task<long> GetSubjectCountAsync()
        {
            var query = _context.Subject.AsQueryable();
            var total = await query.LongCountAsync();
            return total;
        }


        public async Task<long> GetQuizCountForInstructorAsync(long instructorUserId)
        {
            return await _context.Quiz
                .LongCountAsync(q => q.CreatedBy != null && q.CreatedBy.Id == instructorUserId);
        }

        public async Task<double> GetStudentPerformanceAvgAsync(long instructorUserId)
        {
            var attempts = _context.QuizAttempt
                .Include(a => a.Quiz)
                .Where(a => a.IsSubmitted && a.Quiz.TotalMarks > 0 && 
                            a.Quiz.CreatedBy != null && a.Quiz.CreatedBy.Id == instructorUserId);

            if (!await attempts.AnyAsync()) return 0;
            return await attempts.AverageAsync(a => (double)a.TotalScore / a.Quiz.TotalMarks * 100);
        }

        public async Task<double> GetClassPerformanceAvgAsync(long instructorUserId) => await GetStudentPerformanceAvgAsync(instructorUserId);

        public async Task<double> GetEducationMediumPerformanceAvgAsync(long instructorUserId) => await GetStudentPerformanceAvgAsync(instructorUserId);
    }
}
