using QuizSystemModel.BusinessRules;

namespace QuizSystemModel.Models
{
    public class Class
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }
        public QuizSystemUser? ApprovedBy { get; set; }
        public QuizSystemUser? RejectedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        public bool IsApproved { get; set; }
        public ModelStatus Status { get; set; }
        public long? EducationMediumId { get; set; }
        public EducationMedium? EducationMedium { get; set; }
    }
}
