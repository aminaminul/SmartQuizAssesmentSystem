using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class QuestionViewModel
    {
        public long Id { get; set; }

        [Required]
        public long QuizId { get; set; }

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        [Display(Name = "Title")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Question")]
        public string QuestionText { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string OptionA { get; set; } = null!;

        [Required]
        public string OptionB { get; set; } = null!;

        [Required]
        public string OptionC { get; set; } = null!;

        [Required]
        public string OptionD { get; set; } = null!;

        [Required]
        [Display(Name = "Right Option")]
        public string RightOption { get; set; } = null!; 

        [Range(1, 100)]
        public int Marks { get; set; }
    }
}
