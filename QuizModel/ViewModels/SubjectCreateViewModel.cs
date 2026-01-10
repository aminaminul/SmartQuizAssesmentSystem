using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;

namespace SmartQuizAssessmentSystem.ViewModels
{
    public class SubjectCreateViewModel
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "Subject name is required.")]
        [StringLength(30, ErrorMessage = "Subject Name Cannot Be Longer Than 30 Characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please Select An Education Medium.")]
        [Display(Name = "Education Medium")]
        public EducationMediums? EducationMedium { get; set; }

        [Required(ErrorMessage = "Please Select A Class.")]
        [Display(Name = "Class")]
        public long? ClassId { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        public ModelStatus Status { get; set; }

        public IEnumerable<SelectListItem> EducationMediumList { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> ClassList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
