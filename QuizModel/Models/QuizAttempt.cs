using QuizSystemModel.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystemModel.Models
{
    public class QuizAttempt
    {
        public long Id { get; set; }
        public QuizSystemUser? SubmittedBy { get; set; }
        public DateTime SubmittedAt { get; set; }
        public Quiz Quiz { get; set; }
    }
}
