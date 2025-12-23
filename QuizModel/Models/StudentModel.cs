using QuizSystemModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class StudentModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ModelStatus Status { get; set; }
        public QuizSystemUser? User { get; set; }
        public EducationMediumModel? EducationMedium { get; set; }
        public ClassModel? Class { get; set; }
    }
}
