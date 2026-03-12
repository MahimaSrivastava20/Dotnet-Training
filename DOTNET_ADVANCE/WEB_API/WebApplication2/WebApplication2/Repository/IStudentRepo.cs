using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public interface IStudentRepo
    {
        public List<student> GetAll();
        public void Add(student s);
        public void Update(student s);
        public void Delete(int id); 

    }
}
