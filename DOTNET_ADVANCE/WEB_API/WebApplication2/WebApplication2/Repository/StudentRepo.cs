using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class StudentRepo : IStudentRepo
    {
        private readonly StudentDB _context;
        public StudentRepo(StudentDB context)
        {
            _context = context;
        }
        public void Add(student s)
        {
            _context.students.Add(s);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            if (id != null)
            {
                var s = _context.students.Find(id);
                if (s != null)
                {
                    _context.students.Remove(s);
                    _context.SaveChanges();
                }
            }
        }

        public List<student> GetAll()
        {
            return _context.students.ToList();
        }

        public void Update(student s)
        {
            _context.students.Update(s);
            _context.SaveChanges();
        }
    }
}
