using System;
using System.Collections.Generic;
using System.Linq;

namespace University_Course_Registration_System
{
    // =========================
    // University System Class
    // =========================
    public class UniversitySystem
    {
        public Dictionary<string, Course> AvailableCourses { get; private set; }
        public Dictionary<string, Student> Students { get; private set; }
        public List<Student> ActiveStudents { get; private set; }

        public UniversitySystem()
        {
            AvailableCourses = new Dictionary<string, Course>();
            Students = new Dictionary<string, Student>();
            ActiveStudents = new List<Student>();
        }

        public void AddCourse(string code, string name, int credits, int maxCapacity = 50, List<string> prerequisites = null)
        {
            if (AvailableCourses.ContainsKey(code))
                throw new ArgumentException("Course code already exists.");

            AvailableCourses[code] = new Course(code, name, credits, maxCapacity, prerequisites);
        }

        public void AddStudent(string id, string name, string major, int maxCredits = 18, List<string> completedCourses = null)
        {
            if (Students.ContainsKey(id))
                throw new ArgumentException("Student ID already exists.");

            Student student = new Student(id, name, major, maxCredits, completedCourses);
            Students[id] = student;
            ActiveStudents.Add(student);
        }

        public bool RegisterStudentForCourse(string studentId, string courseCode)
        {
            if (!Students.ContainsKey(studentId) || !AvailableCourses.ContainsKey(courseCode))
            {
                Console.WriteLine("Student or Course not found.");
                return false;
            }

            Student student = Students[studentId];
            Course course = AvailableCourses[courseCode];

            if (!student.AddCourse(course))
            {
                Console.WriteLine("Registration failed (Prerequisite/Credit limit/Capacity issue).");
                return false;
            }

            Console.WriteLine("Registration successful!");
            return true;
        }

        public bool DropStudentFromCourse(string studentId, string courseCode)
        {
            if (!Students.ContainsKey(studentId))
                return false;

            return Students[studentId].DropCourse(courseCode);
        }

        public void DisplayAllCourses()
        {
            Console.WriteLine("\nAvailable Courses:");
            foreach (var c in AvailableCourses.Values)
            {
                Console.WriteLine($"{c.CourseCode} | {c.CourseName} | {c.Credits} Credits | {c.GetEnrollmentInfo()}");
            }
        }

        public void DisplayStudentSchedule(string studentId)
        {
            if (!Students.ContainsKey(studentId))
            {
                Console.WriteLine("Student not found.");
                return;
            }

            Students[studentId].DisplaySchedule();
        }

        public void DisplaySystemSummary()
        {
            double avgEnrollment = AvailableCourses.Count == 0
                ? 0
                : AvailableCourses.Values
                    .Average(c => int.Parse(c.GetEnrollmentInfo().Split('/')[0]));

            Console.WriteLine("\nSystem Summary:");
            Console.WriteLine($"Total Students: {Students.Count}");
            Console.WriteLine($"Total Courses: {AvailableCourses.Count}");
            Console.WriteLine($"Average Enrollment: {avgEnrollment:F1}");
        }
    }
}
