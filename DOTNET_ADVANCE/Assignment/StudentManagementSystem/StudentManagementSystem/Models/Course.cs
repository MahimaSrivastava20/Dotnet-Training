using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        public string CourseName { get; set; }

        public int Duration { get; set; }

        public decimal Fees { get; set; }


        // FK
        public int DepartmentId { get; set; }


        // Navigation
        public Department? Department { get; set; }

        public ICollection<Student>? Students { get; set; }
    }
}