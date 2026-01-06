using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IEducationMediumRepository
    {
        Task<List<EducationMedium>> GetAllAsync();
        Task<EducationMedium?> GetByIdAsync(EducationMediums id);
        Task<bool> NameExistsAsync(string name, EducationMediums? excludeId = null);
        Task AddAsync(EducationMedium medium);
        Task UpdateAsync(EducationMedium medium);
        Task DeleteAsync(EducationMedium medium);
    }
}
