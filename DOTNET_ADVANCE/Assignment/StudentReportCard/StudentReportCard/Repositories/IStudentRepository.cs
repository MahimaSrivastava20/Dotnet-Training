using StudentReportCard.Models;

namespace StudentReportCard.Repositories
{
    public interface IStudentRepository
    {
        Student GetStudent(int id, string password);
    }
}
