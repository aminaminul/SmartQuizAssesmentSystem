using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IEducationMediumService
    {
        Task<List<EducationMedium>> GetAllAsync();
        Task<EducationMedium?> GetByIdAsync(EducationMediums id);
        Task<bool> CreateAsync(EducationMedium model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(EducationMediums id, EducationMedium model);
        Task<bool> SoftDeleteAsync(EducationMediums id, QuizSystemUser currentUser);
        Task<bool> ApproveAsync(EducationMediums id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(EducationMediums id, QuizSystemUser currentUser);
        Task<List<Class>> GetClassesByMediumAsync(EducationMediums mediumId);
    }
}
