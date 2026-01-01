using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllAsync(long? classId = null);
        Task<Subject?> GetByIdAsync(long id, bool includeClass = false);
        Task<bool> NameExistsInClassAsync(string name, long? classId, long? excludeId = null);
        Task AddAsync(Subject subject);
        Task UpdateAsync(Subject subject);
    }
}
