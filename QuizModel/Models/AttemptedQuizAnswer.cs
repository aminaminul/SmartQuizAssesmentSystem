using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class AttemptedQuizAnswer
    {
        public long Id { get; set; }
        public QuizAttempt? QuizAttempt { get; set; }
        public QuestionBank? QuestionBank { get; set; }
        public string? SelectedOption { get; set; }
        public decimal Score { get; set; }
    }
}
