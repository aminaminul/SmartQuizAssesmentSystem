using Microsoft.AspNetCore.Mvc.Rendering;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartQuizAssessmentSystem.ViewModels
{
    public class SubjectViewModel
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "Subject name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public long ClassId { get; set; }

        [Required(ErrorMessage = "Education Medium is required")]
        [Display(Name = "Education Medium")]
        public long? EducationMediumId { get; set; }

        public bool IsApproved { get; set; }

        public ModelStatus Status { get; set; } = ModelStatus.Pending;

        public IEnumerable<SelectListItem> EducationMediumList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        
        public Class? Class { get; set; }
        public EducationMedium? EducationMedium { get; set; }
    }
}
