using Microsoft.AspNetCore.Identity;

namespace QuizSystemModel.Models
{
    public class QuizSystemUser : IdentityUser<long>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
