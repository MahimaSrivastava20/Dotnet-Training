using System.ComponentModel.DataAnnotations;

namespace StudentReportCard.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Course { get; set; }

        public string Password { get; set; }

        public int Maths { get; set; }
        public int Science { get; set; }
        public int English { get; set; }

        public string Photo { get; set; }   // NEW PROPERTY
    }
}