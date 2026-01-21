using QuizSystemModel.Models;

public interface ISubjectService
{
    Task<List<Subject>> GetAllAsync(long? classId = null, long? educationMediumId = null);
    Task<Subject?> GetByIdAsync(long id, bool includeClass = false);
    Task<bool> CreateAsync(Subject model, QuizSystemUser currentUser);
    Task<bool> UpdateAsync(long id, Subject model, QuizSystemUser currentUser);
    Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
    Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
    Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
    Task<List<Subject>> GetPendingAsync();
}
