namespace StudentManagementSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public int DepartmentId { get; set; }

        public int CourseId { get; set; }

        public Department? Department { get; set; }

        public Course? Course { get; set; }
    }
}
