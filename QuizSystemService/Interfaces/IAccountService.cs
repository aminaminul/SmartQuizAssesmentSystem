using Microsoft.AspNetCore.Identity;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterStudentAsync(StudentAddViewModel model);
        Task<IdentityResult> RegisterInstructorAsync(InstructorAddViewModel model);
    }
}
