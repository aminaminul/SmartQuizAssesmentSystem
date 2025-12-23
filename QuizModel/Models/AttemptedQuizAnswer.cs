using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class AttemptedQuizAnswerModel
    {
        public long Id { get; set; }
        public QuizAttemptModel? QuizAttempt { get; set; }
        public QuestionBankModel? QuestionBank { get; set; }
        public string? SelectedOption { get; set; }
        public decimal Score { get; set; }
    }
}
