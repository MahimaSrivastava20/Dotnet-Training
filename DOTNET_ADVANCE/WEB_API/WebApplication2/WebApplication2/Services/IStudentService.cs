using WebApplication2.Models;
namespace WebApplication2.Services
{
    public interface IStudentService
    {
        public List<student> GetStudent();
        public void AddStudent(student s);
        public void UpdateStudent(student s);
        public void DeleteStudent(int studentId);
    }
}
