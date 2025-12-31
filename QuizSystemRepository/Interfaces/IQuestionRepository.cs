using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<QuestionBank>> GetByQuizAsync(long quizId, string? subject = null);
        Task<QuestionBank?> GetByIdAsync(long id);
        Task AddAsync(QuestionBank question);
        Task UpdateAsync(QuestionBank question);
        Task DeleteAsync(QuestionBank question);
    }
}
