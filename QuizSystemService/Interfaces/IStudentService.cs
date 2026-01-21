using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync(long? classId = null);
        Task<Student?> GetByIdAsync(long id);
        Task<Student?> GetForEditAsync(long id);
        Task<bool> CreateAsync(StudentAddViewModel model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Student model, long? educationMediumId, long? classId);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);
        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task<List<Class>> GetClassesAsync(long? mediumId = null);
    }
}
