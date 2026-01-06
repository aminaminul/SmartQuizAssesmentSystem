using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;

public interface IClassRepository
{
    Task<List<Class>> GetAllAsync(EducationMediums? educationMediumId = null);
    Task<Class?> GetByIdAsync(long id, bool includeMedium = false);
    Task<bool> NameExistsInMediumAsync(string name,EducationMediums educationMedium,long? excludeId = null);
    Task<List<Class>> GetByMediumAsync(EducationMediums medium);
    Task AddAsync(Class cls);
    Task UpdateAsync(Class cls);
    Task<List<Class>> GetPendingAsync();
}
