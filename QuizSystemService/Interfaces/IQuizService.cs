using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IQuizService
    {
        Task<List<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(long id, bool includeQuestions = false);

        Task<bool> CreateAsync(Quiz quiz, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Quiz quiz, QuizSystemUser currentUser);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);

        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
    }
}
