using Microsoft.EntityFrameworkCore;
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
            var query = _context.Quiz.Where(q => !q.IsApproved);
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



    }

}
