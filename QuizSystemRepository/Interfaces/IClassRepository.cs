using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IClassRepository
    {
        Task<List<Class>> GetAllAsync(long? educationMediumId = null);
        Task<Class?> GetByIdAsync(long id, bool includeMedium = false);
        Task<bool> NameExistsInMediumAsync(ClassNameEnum className, long educationMediumId, long? excludeId = null);
        Task<List<Class>> GetByMediumAsync(long mediumId);
        Task AddAsync(Class cls);
        Task UpdateAsync(Class cls);
        Task<List<Class>> GetPendingAsync();
    }
}
