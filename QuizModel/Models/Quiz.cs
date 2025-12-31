using QuizSystemModel.BusinessRules;
namespace QuizSystemModel.Models
{
    public class Quiz
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }

        public bool IsApproved { get; set; }

        public QuizSystemUser? ApprovedBy { get; set; }
        public QuizSystemUser? RejectedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        public ModelStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public DateTime? Duration { get; set; }
        public int TotalMarks { get; set; }
        public decimal RequiredPassPercentage { get; set; }
        public string Subject { get; set; }
        public ICollection<QuestionBank> Questions { get; set; } = new List<QuestionBank>();
    }
}
