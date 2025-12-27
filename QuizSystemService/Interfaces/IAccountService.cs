using Microsoft.AspNetCore.Identity;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
    }
}
