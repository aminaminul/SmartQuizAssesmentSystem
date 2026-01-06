using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystemService.Interfaces;

namespace SmartQuizAssessmentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProfileApprovalController : Controller
    {
        private readonly IProfileUpdateService _profileService;

        public ProfileApprovalController(IProfileUpdateService profileService)
        {
            _profileService = profileService;
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
            await _profileService.ApproveProfileUpdateAsync(id, 0);
            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string? comment)
        {
            await _profileService.RejectProfileUpdateAsync(id, 0, comment);
            return RedirectToAction(nameof(Pending));
        }
    }
}
