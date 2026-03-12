using WebApplication2.Repository;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepo;
        public StudentService(IStudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }
        public void AddStudent(student s)
        {
            _studentRepo.Add(s);
        }

        public void DeleteStudent(int studentId)
        {
            _studentRepo.Delete(studentId);
        }

        public List<student> GetStudent()
        {
           return _studentRepo.GetAll ();
        }

        public void UpdateStudent(student s)
        {
            _studentRepo.Update(s);
        }
    }
}
