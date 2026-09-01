using System.Collections.Generic;
using System.Threading.Tasks;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardAsync();
        Task<GlobalSearchViewModel> SearchAsync(string query);
        Task<List<QuizAttempt>> GetTopStudentsAsync(int count);
    }
}
