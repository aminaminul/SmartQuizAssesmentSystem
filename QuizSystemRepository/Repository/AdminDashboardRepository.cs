using System.Collections.Generic;
using System.Threading.Tasks;
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
                .Where(p => p.Status == ProfileUpdateStatus.Pending)
                .LongCountAsync();
        }

        public async Task<List<Instructor>> SearchInstructorsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Instructor>();

            query = query.Trim();
            return await _context.Instructor
                .Where(i => (i.FirstName != null && i.FirstName.Contains(query)) ||
                            (i.LastName != null && i.LastName.Contains(query)) ||
                            (i.Email != null && i.Email.Contains(query)))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Student>> SearchStudentsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Student>();

            query = query.Trim();
            return await _context.Student
                .Where(s => (s.FirstName != null && s.FirstName.Contains(query)) ||
                            (s.LastName != null && s.LastName.Contains(query)) ||
                            (s.Email != null && s.Email.Contains(query)))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Quiz>> SearchQuizzesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Quiz>();

            query = query.Trim();
            return await _context.Quiz
                .Where(q => (q.Name != null && q.Name.Contains(query)) ||
                            (q.Description != null && q.Description.Contains(query)))
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<Subject>> SearchSubjectsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Subject>();

            query = query.Trim();
            return await _context.Subject
                .Where(s => s.Name != null && s.Name.Contains(query))
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

        public async Task<List<QuizAttempt>> GetTopPerformingStudentsAsync(int count)
        {
            return await _context.QuizAttempt
                .Include(a => a.StudentUser)
                .Include(a => a.Quiz)
                .Where(a => a.IsSubmitted)
                .OrderByDescending(a => a.TotalScore)
                .Take(count)
                .ToListAsync();
        }
    }

}
