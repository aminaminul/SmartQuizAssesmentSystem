using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using QuizSystemRepository.Interfaces;

namespace QuizSystemRepository.Repositories
{
    public class QuizAttemptRepository : IQuizAttemptRepository
    {
        private readonly AppDbContext _context;

        public QuizAttemptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QuizAttempt?> GetByIdAsync(long id)
        {
            return await _context.QuizAttempt
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.QuestionBank)
                .Include(a => a.StudentUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(QuizAttempt attempt)
        {
            await _context.QuizAttempt.AddAsync(attempt);
        }

        public Task UpdateAsync(QuizAttempt attempt)
        {
            _context.QuizAttempt.Update(attempt);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
