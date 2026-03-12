using StudentReportCard.Models;

namespace StudentReportCard.Services
{
    public interface IStudentService
    {
        Student Login(int id, string password);
    }
}
