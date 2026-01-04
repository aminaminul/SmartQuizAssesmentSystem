using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IQuizService
    {
        Task<List<Quiz>> GetAllAsync();
        Task<Quiz?> GetEntityAsync(long id, bool includeQuestions = false);
        Task<QuizViewModel?> GetForEditAsync(long id);
        Task<bool> CreateAsync(QuizViewModel model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, QuizViewModel model, QuizSystemUser currentUser);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
        Task<List<Quiz>> GetPendingAsync();
    }
}
