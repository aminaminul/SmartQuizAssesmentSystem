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
            var student = await _context.Student
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == studentUserId);

            if (student == null) return new List<Quiz>();

            return await _context.Quiz
                .Include(q => q.Subject)
                .Where(q => q.Status == ModelStatus.Active
                            && q.IsApproved
                            && q.Questions.Any(ques => ques.Status == ModelStatus.Active)
                            && q.EducationMediumId == student.EducationMediumId
                            && q.ClassId == student.ClassId
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
