using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(long id, bool includeUser = false);

        Task<bool> CreateAsync(StudentAddView model, QuizSystemUser currentUser);
        Task<bool> UpdateAsync(long id, Student model, long? educationMediumId, long? classId);
        Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser);

        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task<List<Class>> GetClassesAsync();
    }
}
