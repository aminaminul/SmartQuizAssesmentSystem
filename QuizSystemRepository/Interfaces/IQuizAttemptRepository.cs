using System.Threading.Tasks;
using QuizSystemModel.Models;

namespace QuizSystemRepository.Interfaces
{
    public interface IQuizAttemptRepository
    {
        Task<QuizAttempt?> GetByIdAsync(long id);
        Task<QuizAttempt?> GetByUserAndQuizAsync(long studentUserId, long quizId);
        Task AddAsync(QuizAttempt attempt);
        Task UpdateAsync(QuizAttempt attempt);
        Task SaveChangesAsync();
        Task<List<QuizAttempt>> GetAllAttemptsAsync();
    }
}