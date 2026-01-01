using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IAccountRepository
    {
        Task AddStudentAsync(Student student);
        Task AddInstructorAsync(Instructor instructor);

        Task<bool> StudentEmailExistsAsync(string email);
        Task<bool> StudentPhoneExistsAsync(string phone);

        Task<bool> InstructorEmailExistsAsync(string email);
        Task<bool> InstructorPhoneExistsAsync(string phone);
    }
}
