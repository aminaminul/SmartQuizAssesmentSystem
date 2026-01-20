using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class QuestionViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Quiz selection is required")]
        public long QuizId { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Subject name must be between 2 and 50 characters")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Question Title is required")]
        [Display(Name = "Title")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Question Text is required")]
        [Display(Name = "Question")]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Question must be at least 5 characters long")]
        public string QuestionText { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Option A is required")]
        [Display(Name = "Option A")]
        [StringLength(1000, ErrorMessage = "Option cannot exceed 1000 characters")]
        public string OptionA { get; set; } = null!;

        [Required(ErrorMessage = "Option B is required")]
        [Display(Name = "Option B")]
        [StringLength(1000, ErrorMessage = "Option cannot exceed 1000 characters")]
        public string OptionB { get; set; } = null!;

        [Required(ErrorMessage = "Option C is required")]
        [Display(Name = "Option C")]
        [StringLength(1000, ErrorMessage = "Option cannot exceed 1000 characters")]
        public string OptionC { get; set; } = null!;

        [Required(ErrorMessage = "Option D is required")]
        [Display(Name = "Option D")]
        [StringLength(1000, ErrorMessage = "Option cannot exceed 1000 characters")]
        public string OptionD { get; set; } = null!;

        [Required(ErrorMessage = "Correct option must be selected")]
        [Display(Name = "Right Option")]
        [RegularExpression(@"^[A-D]$", ErrorMessage = "Only Options A, B, C, or D are valid")]
        public string RightOption { get; set; } = null!; 

        [Required(ErrorMessage = "Marks are required")]
        [Range(1, 100, ErrorMessage = "Marks must be between 1 and 100")]
        public int Marks { get; set; }
    }
}
