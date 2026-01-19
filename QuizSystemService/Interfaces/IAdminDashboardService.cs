using QuizSystemModel.ViewModels;

namespace QuizSystemService.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardAsync();
        Task<GlobalSearchViewModel> SearchAsync(string query);
    }
}
