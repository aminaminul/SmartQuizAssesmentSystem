using QuizSystemModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class Instructor
    {
        public long Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ModelStatus Status { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? HscPassingInstrutute { get; set; }
        public long? HscPassingYear { get; set; }
        public string? HscGrade { get; set; }
        public long? UserId { get; set; }
        public QuizSystemUser? User { get; set; }
        public long? EducationMediumId { get; set; }
        public EducationMedium? EducationMedium { get; set; }
        public long? ClassId { get; set; }
        public Class? Class { get; set; }
    }
}
