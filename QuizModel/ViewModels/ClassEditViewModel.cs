using System.ComponentModel.DataAnnotations;
using QuizSystemModel.BusinessRules;

namespace QuizSystemModel.ViewModels
{
    public class ClassEditViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Please select a class")]
        [Display(Name = "Class")]
        public long ClassId { get; set; }

        [Required(ErrorMessage = "Please select an education medium")]
        [Display(Name = "Education Medium")]
        public long EducationMediumId { get; set; }

        [Required]
        public ModelStatus Status { get; set; }
    }
}
