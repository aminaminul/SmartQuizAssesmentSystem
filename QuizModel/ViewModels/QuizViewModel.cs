using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class QuizViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Subject")]
        public long? SubjectId { get; set; }

        [Required]
        [Display(Name = "Class")]
        public long? ClassId { get; set; }

        [Required]
        [Display(Name = "Education Medium")]
        public long? EducationMediumId { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Display(Name = "Start At")]
        public DateTime? StartAt { get; set; }

        [Display(Name = "End At")]
        public DateTime? EndAt { get; set; }

        [Display(Name = "Duration (minutes)")]
        [Range(1, 600)]
        public int? DurationMinutes { get; set; }

        [Display(Name = "Total Marks")]
        [Range(1, 1000)]
        public int TotalMarks { get; set; }

        [Display(Name = "Required Pass Percentage")]
        [Range(0, 100)]
        public decimal RequiredPassPercentage { get; set; }
    }
}
