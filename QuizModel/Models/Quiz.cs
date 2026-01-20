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
        public TimeSpan? Duration { get; set; }
        public int TotalMarks { get; set; }
        public decimal NegativeMarking { get; set; }
        public decimal RequiredPassPercentage { get; set; }
        public long? SubjectId { get; set; }
        public virtual Subject? Subject { get; set; }

        public long? ClassId { get; set; }
        public virtual Class? Class { get; set; }

        public long? EducationMediumId { get; set; }
        public virtual EducationMedium? EducationMedium { get; set; }
        public ICollection<QuestionBank> Questions { get; set; } = new List<QuestionBank>();
    }
}
