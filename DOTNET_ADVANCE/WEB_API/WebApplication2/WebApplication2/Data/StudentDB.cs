using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
namespace WebApplication2.Data
{

    public class StudentDB: DbContext
    {
        public StudentDB(DbContextOptions<StudentDB> options) : base(options)
        {
        }

        public DbSet<student> students { get; set; }
    }
}
