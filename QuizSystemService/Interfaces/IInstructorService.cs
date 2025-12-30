using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Services
{
    public interface IInstructorService
    {
        Task<List<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(long id);
        Task<Instructor?> GetForEditAsync(long id);
        Task<bool> CreateAsync(InstructorAddViewModel model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Instructor model, long? educationMediumId);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<List<EducationMedium>> GetEducationMediumsAsync();
    }
}
