using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IEducationMediumService
    {
        Task<List<EducationMedium>> GetAllAsync(long? id = null);
        Task<EducationMedium?> GetByIdAsync(long id);
        Task<bool> CreateAsync(EducationMedium model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, EducationMedium model);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
        Task<List<Class>> GetClassesByMediumAsync(long mediumId);
        Task<List<EducationMedium>> GetPendingAsync();
    }
}
