using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class BatchQuestionViewModel
    {
        [Required(ErrorMessage = "Quiz selection is required")]
        public long QuizId { get; set; }

        public string? Subject { get; set; }

        [Range(1, 50, ErrorMessage = "Question count must be between 1 and 50")]
        public int QuestionCount { get; set; } = 1;

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
