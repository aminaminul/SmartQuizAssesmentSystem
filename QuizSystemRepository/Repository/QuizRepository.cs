using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _context;

        public QuizRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Quiz>> GetAllAsync()
        {
            return _context.Quiz
                .Include(q => q.Subject)
                .Include(q => q.Class)
                .Include(q => q.EducationMedium)
                .Where(q => q.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

        public Task<Quiz?> GetByIdAsync(long id, bool includeQuestions = false)
        {
            var query = _context.Quiz
                .Include(q => q.Subject)
                .Include(q => q.Class)
                .Include(q => q.EducationMedium)
                .AsQueryable();

            if (includeQuestions)
                query = query.Include(q => q.Questions);

            return query.FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task AddAsync(Quiz quiz)
        {
            _context.Quiz.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            _context.Quiz.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Quiz quiz)
        {
            _context.Quiz.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Quiz>> GetAvailableForStudentAsync(long studentUserId, DateTime now)
        {
            var student = await _context.Student
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == studentUserId);

            if (student == null) return new List<Quiz>();

            return await _context.Quiz
                .Where(q => q.IsApproved
                            && q.Status == ModelStatus.Active
                            && q.EducationMediumId == student.EducationMediumId
                            && q.ClassId == student.ClassId
                            && (q.StartAt == null || q.StartAt <= now)
                            && (q.EndAt == null || q.EndAt >= now))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Quiz?> GetByIdWithQuestionsAsync(long quizId)
        {
            return await _context.Quiz
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }
        public Task<List<Quiz>> GetPendingAsync()
        {
            return _context.Quiz
                .Where(q => (q.Status == ModelStatus.Pending || q.Status == ModelStatus.InActive) && q.Status != ModelStatus.Deleted)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public Task<List<Quiz>> GetByClassAsync(long classId)
        {
            return _context.Quiz
                .Include(q => q.Subject)
                .Include(q => q.Class)
                .Include(q => q.EducationMedium)
                .Where(q => q.ClassId == classId && q.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

    }
}
