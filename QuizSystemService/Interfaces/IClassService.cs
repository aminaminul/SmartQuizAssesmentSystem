using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IClassService
    {
        Task<List<Class>> GetAllAsync(long? educationMediumId = null);
        Task<Class?> GetByIdAsync(long id, bool includeMedium = false);
        Task<bool> CreateAsync(Class model, long educationMediumId, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Class model);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<List<Class>> GetPendingAsync();
        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
    }
}
