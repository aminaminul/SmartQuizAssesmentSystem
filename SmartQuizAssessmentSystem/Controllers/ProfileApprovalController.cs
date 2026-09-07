using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileApprovalController : Controller
    {
        private readonly IProfileUpdateService _profileService;
        private readonly UserManager<QuizSystemUser> _userManager;

        public ProfileApprovalController(
            IProfileUpdateService profileService,
            UserManager<QuizSystemUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var requests = await _profileService.GetPendingRequestsAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var adminId = currentUser?.Id ?? 0;
            await _profileService.ApproveProfileUpdateAsync(id, adminId);
            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string? comment)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var adminId = currentUser?.Id ?? 0;
            await _profileService.RejectProfileUpdateAsync(id, adminId, comment);
            return RedirectToAction(nameof(Pending));
        }
    }
}
