using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext _context;

        public QuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<QuestionBank>> GetByQuizAsync(long quizId, string? subject = null)
        {
            var query = _context.QuestionBank
                .Where(q => q.QuizId == quizId && q.Status != ModelStatus.Deleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(subject))
                query = query.Where(q => q.Subject == subject);

            return query.ToListAsync();
        }

        public Task<QuestionBank?> GetByIdAsync(long id)
        {
            return _context.QuestionBank
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task AddAsync(QuestionBank question)
        {
            _context.QuestionBank.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionBank question)
        {
            _context.QuestionBank.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(QuestionBank question)
        {
            _context.QuestionBank.Remove(question);
            await _context.SaveChangesAsync();
        }
    }
}
