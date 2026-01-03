using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using QuizSystemRepository.Interfaces;

namespace QuizSystemRepository.Repositories
{
    public class AttemptedQuizAnswerRepository : IAttemptedQuizAnswerRepository
    {
        private readonly AppDbContext _context;

        public AttemptedQuizAnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttemptedQuizAnswer>> GetByAttemptIdAsync(long attemptId)
        {
            return await _context.AttemptedQuizAnswer
                .Include(a => a.QuestionBank)
                .Where(a => a.QuizAttemptId == attemptId)
                .ToListAsync();
        }

        public async Task<AttemptedQuizAnswer?> GetByAttemptAndQuestionAsync(long attemptId, long questionId)
        {
            return await _context.AttemptedQuizAnswer
                .FirstOrDefaultAsync(a => a.QuizAttemptId == attemptId &&
                                          a.QuestionBankId == questionId);
        }

        public async Task AddAsync(AttemptedQuizAnswer answer)
        {
            await _context.AttemptedQuizAnswer.AddAsync(answer);
        }

        public Task UpdateAsync(AttemptedQuizAnswer answer)
        {
            _context.AttemptedQuizAnswer.Update(answer);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
