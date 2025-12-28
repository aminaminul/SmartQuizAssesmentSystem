using QuizSystemModel.BusinessRules;
using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class ClassMediumView
    {
        [Required]
        [Display(Name = "Class")]
        public SelectClass Class { get; set; }

        [Required]
        [Display(Name = "Education Medium")]
        public EducationMediumType Medium { get; set; }
    }
}
