using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionBank>> GetByQuizAsync(long quizId, string? subject = null);
        Task<QuestionBank?> GetByIdAsync(long id);
        Task<bool> CreateAsync(QuestionBank question, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, QuestionBank question, QuizSystemUser currentUser);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
    }
}
