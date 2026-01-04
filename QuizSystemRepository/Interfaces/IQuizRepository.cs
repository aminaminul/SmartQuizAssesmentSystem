using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IQuizRepository
    {
        Task<List<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(long id, bool includeQuestions = false);
        Task AddAsync(Quiz quiz);
        Task UpdateAsync(Quiz quiz);
        Task DeleteAsync(Quiz quiz);
        Task<List<Quiz>> GetPendingAsync();
        Task<List<Quiz>> GetAvailableForStudentAsync(long studentUserId, DateTime now);
        Task<Quiz?> GetByIdWithQuestionsAsync(long quizId);
    }
}
