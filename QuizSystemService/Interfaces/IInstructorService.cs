using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IInstructorService
    {
        Task<List<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(long id);
        Task<Instructor?> GetForEditAsync(long id);
        Task<bool> CreateAsync(InstructorAddViewModel model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Instructor model, long? educationMediumId, long? classId);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<bool> ApproveAsync(long id, QuizSystemUser currentUser);
        Task<bool> RejectAsync(long id, QuizSystemUser currentUser);
        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task<List<Class>> GetClassesAsync();
        Task<Instructor?> GetByUserIdAsync(long userId);
        Task<List<Instructor>> GetPendingAsync();
    }
}
