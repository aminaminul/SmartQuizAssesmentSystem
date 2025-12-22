using Microsoft.AspNetCore.Identity;

namespace SmartQuizAssessmentSystem.Models
{
    public class QuizSystemUser : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Role { get; set; }
   
    }
    public class Instructor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Qualifications { get; set; }
        public string Expertise { get; set; }
        public string Phone { get; set; }
        public QuizSystemUser User { get; set; }
    }
}
