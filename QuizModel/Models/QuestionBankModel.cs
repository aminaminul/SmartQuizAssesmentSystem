using QuizSystemModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class QuestionBankModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public QuizSystemUser? CreatedBy { get; set; }
        public QuizSystemUser? ModifiedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ModelStatus Status { get; set; }
        public QuizModel Quiz { get; set; }
        public string QuestionText { get; set; }
        public string Description { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }  
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string RightOption { get; set; }
        public int Markes { get; set; }

    }
}
