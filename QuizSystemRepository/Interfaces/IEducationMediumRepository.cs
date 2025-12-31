using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IEducationMediumRepository
    {
        Task<List<EducationMedium>> GetAllAsync();
        Task<EducationMedium?> GetByIdAsync(long id);
        Task<bool> NameExistsAsync(string name, long? excludeId = null);

        Task AddAsync(EducationMedium medium);
        Task UpdateAsync(EducationMedium medium);
        Task DeleteAsync(EducationMedium medium);

        Task<List<Class>> GetClassesByMediumAsync(long mediumId);
    }
}
