using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface ISubjectService
    {
        Task<List<Subject>> GetAllAsync(long? classId = null);
        Task<Subject?> GetByIdAsync(long id, bool includeClass = false);
        Task<bool> CreateAsync(Subject model, long? classId, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Subject model, long? classId);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);

        Task<List<Subject>> GetPendingAsync();
    }
}
