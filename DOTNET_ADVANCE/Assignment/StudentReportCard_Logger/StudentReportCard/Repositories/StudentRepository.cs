using StudentReportCard.Data;
using StudentReportCard.Models;

namespace StudentReportCard.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Student GetStudent(int id, string password)
        {
            return _context.Students
                .FirstOrDefault(s => s.Id == id && s.Password == password);
        }
    }
}
