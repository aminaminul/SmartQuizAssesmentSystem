using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class ClassCreateViewModel
    {
        [Required(ErrorMessage = "Please select a class")]
        [Display(Name = "Class")]
        public long ClassId { get; set; }

        [Required(ErrorMessage = "Please select an education medium")]
        [Display(Name = "Education Medium")]
        public long EducationMediumId { get; set; }
    }
}
