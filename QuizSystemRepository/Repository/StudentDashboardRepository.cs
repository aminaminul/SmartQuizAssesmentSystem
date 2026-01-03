using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class StudentDashboardRepository : IStudentDashboardRepository
    {
        private readonly AppDbContext _context;

        public StudentDashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Quiz>> GetAvailableQuizzesAsync(long studentUserId, DateTime now)
        {
            return await _context.Quiz
                .Where(q => q.IsApproved
                            && q.Status == ModelStatus.Active
                            && (q.StartAt == null || q.StartAt <= now)
                            && (q.EndAt == null || q.EndAt >= now))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<QuizAttempt>> GetStudentAttemptsAsync(long studentUserId)
        {
            return await _context.QuizAttempt
                .Include(a => a.Quiz)
                .Where(a => a.StudentUserId == studentUserId)
                .OrderByDescending(a => a.StartAt)
                .ToListAsync();
        }
    }
}
