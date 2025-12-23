using Microsoft.AspNetCore.Identity;

namespace QuizSystemModel.Models
{
    public class QuizSystemUser : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
