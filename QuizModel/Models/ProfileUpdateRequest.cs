using QuizSystemModel.BusinessRules;

namespace QuizSystemModel.Models
{
    public class ProfileUpdateRequest
    {
        public long Id { get; set; }
        public long UserId { get; set; }  
        public string Role { get; set; } = null!;

        public string OldDataJson { get; set; } = null!;
        public string NewDataJson { get; set; } = null!;

        public ProfileUpdateStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? AdminComment { get; set; }
        public DateTime? LastAppliedAt { get; set; }
    }
}
