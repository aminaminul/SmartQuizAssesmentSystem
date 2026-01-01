using QuizSystemModel.BusinessRules;
namespace QuizSystemModel.Models
{
    public class QuestionBank
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ModelStatus Status { get; set; }
        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public string QuestionText { get; set; }
        public string Description { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }  
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string RightOption { get; set; }
        public int Marks { get; set; }
        public string Subject { get; set; }
    }
}