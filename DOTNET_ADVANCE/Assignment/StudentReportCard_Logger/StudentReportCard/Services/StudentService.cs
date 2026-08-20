using StudentReportCard.Models;
using StudentReportCard.Repositories;

namespace StudentReportCard.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;

        public StudentService(IStudentRepository repo)
        {
            _repo = repo;
        }

        public Student Login(int id, string password)
        {
            return _repo.GetStudent(id, password);
        }
    }
}
