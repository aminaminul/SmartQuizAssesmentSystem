using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly AppDbContext _context;

        public AdminDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<long> GetInstructorCountAsync()
        {
            var query = _context.Instructor.AsQueryable();
            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetStudentCountAsync()
        {
            var query = _context.Student.AsQueryable();
            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetQuizCountAsync()
        {
            var query = _context.Quiz.AsQueryable();
            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetPendingQuizCountAsync()
        {
            var query = _context.Quiz.Where(q => q.Status == ModelStatus.Pending || q.Status == ModelStatus.InActive);
            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetClassCountAsync()
        {
            var query = _context.Class.AsQueryable();
            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetSubjectCountAsync()
        {
            var query = _context.Subject.AsQueryable();
            var count = await query.LongCountAsync();
            return count;
        }
        public async Task<long> GetEducationMediumCountAsync()
        {
            IQueryable<EducationMedium> query = _context.EducationMedium;

            long total = await query.LongCountAsync();

            return total;
        }

        public async Task<long> GetPendingClassCountAsync()
        {
            var query = _context.Class
                .Where(c => c.Status == ModelStatus.Pending || c.Status == ModelStatus.InActive);

            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetPendingSubjectCountAsync()
        {
            var query = _context.Subject
                .Where(s => s.Status == ModelStatus.Pending || s.Status == ModelStatus.InActive);

            var count = await query.LongCountAsync();
            return count;
        }

        public async Task<long> GetPendingInstructorCountAsync()
        {
            return await _context.Instructor
                .Where(i => i.Status == ModelStatus.Pending || i.Status == ModelStatus.InActive)
                .LongCountAsync();
        }

        public async Task<long> GetPendingEducationMediumCountAsync()
        {
            return await _context.EducationMedium
                .Where(m => m.Status == ModelStatus.Pending || m.Status == ModelStatus.InActive)
                .LongCountAsync();
        }

        public async Task<long> GetPendingProfileUpdateCountAsync()
        {
            return await _context.ProfileUpdateRequests
                .Where(p => p.Status == ProfileUpdateStatus.Pending || p.Status == ProfileUpdateStatus.Rejected)
                .LongCountAsync();
        }

        public async Task<List<Instructor>> SearchInstructorsAsync(string query)
        {
            return await _context.Instructor
                .Where(i => i.FirstName!.Contains(query) || i.LastName!.Contains(query) || i.Email!.Contains(query))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Student>> SearchStudentsAsync(string query)
        {
            return await _context.Student
                .Where(s => s.FirstName!.Contains(query) || s.LastName!.Contains(query) || s.Email!.Contains(query))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Quiz>> SearchQuizzesAsync(string query)
        {
            return await _context.Quiz
                .Where(q => q.Name.Contains(query) || q.Description.Contains(query))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Subject>> SearchSubjectsAsync(string query)
        {
            return await _context.Subject
                .Where(s => s.Name.Contains(query))
                .Take(20)
                .ToListAsync();
        }


        public async Task<double> GetStudentPerformanceAvgAsync()
        {
            var attempts = _context.QuizAttempt
                .Include(a => a.Quiz)
                .Where(a => a.IsSubmitted && a.Quiz.TotalMarks > 0);

            if (!await attempts.AnyAsync()) return 0;
            return await attempts.AverageAsync(a => (double)a.TotalScore / a.Quiz.TotalMarks * 100);
        }

        public async Task<double> GetClassPerformanceAvgAsync() => await GetStudentPerformanceAvgAsync();

        public async Task<double> GetEducationMediumPerformanceAvgAsync() => await GetStudentPerformanceAvgAsync();
    }

}
