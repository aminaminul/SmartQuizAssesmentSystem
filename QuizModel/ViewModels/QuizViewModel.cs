using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class QuizViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Quiz Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Quiz Name must be between 3 and 100 characters")]
        [Display(Name = "Quiz Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Subject is required")]
        [Display(Name = "Subject")]
        public long? SubjectId { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public long? ClassId { get; set; }

        [Required(ErrorMessage = "Education Medium is required")]
        [Display(Name = "Education Medium")]
        public long? EducationMediumId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
        public string Description { get; set; } = null!;

        [Display(Name = "Start Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime? StartAt { get; set; }

        [Display(Name = "End Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime? EndAt { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int? DurationMinutes { get; set; }

        [Display(Name = "Total Marks")]
        [Required(ErrorMessage = "Total marks are required")]
        [Range(1, 1000, ErrorMessage = "Total marks must be between 1 and 1000")]
        public int TotalMarks { get; set; }

        [Display(Name = "Negative Marking")]
        [Required(ErrorMessage = "Negative marking is required")]
        [Range(0, 100, ErrorMessage = "Negative marking must be between 0 and 100")]
        public decimal NegativeMarking { get; set; }

        [Display(Name = "Required Pass Percentage")]
        [Required(ErrorMessage = "Pass percentage is required")]
        [Range(0, 100, ErrorMessage = "Pass percentage must be between 0 and 100")]
        public decimal RequiredPassPercentage { get; set; }
    }
}
