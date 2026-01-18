using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class ClassCreateViewModel
    {
        [Required(ErrorMessage = "Please select a class")]
        [Range(1, 12, ErrorMessage = "Please select a valid class (1-12)")]
        public long ClassId { get; set; }

        [Required(ErrorMessage = "Please select an education medium")]
        public long EducationMediumId { get; set; }
    }
}
