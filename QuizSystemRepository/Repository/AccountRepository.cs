using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentAsync(Student student)
        {
            await _context.Student.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task AddInstructorAsync(Instructor instructor)
        {
            await _context.Instructor.AddAsync(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> StudentEmailExistsAsync(string email)
        {
            return await _context.Student
                .AnyAsync(s => s.Email != null && s.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> StudentPhoneExistsAsync(string phone)
        {
            return await _context.Student
                .AnyAsync(s => s.PhoneNumber != null && s.PhoneNumber == phone);
        }

        public async Task<bool> InstructorEmailExistsAsync(string email)
        {
            return await _context.Instructor
                .AnyAsync(i => i.Email != null && i.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> InstructorPhoneExistsAsync(string phone)
        {
            return await _context.Instructor
                .AnyAsync(i => i.PhoneNumber != null && i.PhoneNumber == phone);
        }
    }
}
