using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionBank>> GetByQuizAsync(long quizId, string? subject = null);
        Task<QuestionBank?> GetEntityAsync(long id);

        Task<QuestionViewModel?> GetForEditAsync(long id);
        Task<bool> CreateAsync(QuestionViewModel model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, QuestionViewModel model, QuizSystemUser currentUser);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
    }
}
