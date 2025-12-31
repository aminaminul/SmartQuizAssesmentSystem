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
                .Where(q => q.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

        public Task<Quiz?> GetByIdAsync(long id, bool includeQuestions = false)
        {
            var query = _context.Quiz.AsQueryable();

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
    }
}
