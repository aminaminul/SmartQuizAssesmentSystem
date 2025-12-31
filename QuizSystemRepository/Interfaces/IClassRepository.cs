using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync(long? educationMediumId = null);
        Task<Class?> GetByIdAsync(long id, bool includeMedium = false);
        Task<bool> NameExistsInMediumAsync(string name, long educationMediumId, long? excludeId = null);
        Task AddAsync(Class cls);
        Task UpdateAsync(Class cls);
        Task DeleteAsync(Class cls);

        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task SaveChangesAsync();
    }
}
