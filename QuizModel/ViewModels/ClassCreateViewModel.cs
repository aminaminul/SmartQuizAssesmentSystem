using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class ClassCreateViewModel
    {
        [Required(ErrorMessage = "Please select a class")]
        public long ClassId { get; set; }

        [Required(ErrorMessage = "Please select an education medium")]
        public long EducationMediumId { get; set; }
    }
}
