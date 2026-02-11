using System;
using System.Collections.Generic;
using System.Linq;

namespace University_Course_Registration_System
{
     // =========================
    // Program (Menu-Driven)
    // =========================
    class Program
    {
        static void Main()
        {
            UniversitySystem system = new UniversitySystem();
            bool exit = false;

            Console.WriteLine("Welcome to University Course Registration System");

            while (!exit)
            {
                Console.WriteLine("\n1. Add Course");
                Console.WriteLine("2. Add Student");
                Console.WriteLine("3. Register Student for Course");
                Console.WriteLine("4. Drop Student from Course");
                Console.WriteLine("5. Display All Courses");
                Console.WriteLine("6. Display Student Schedule");
                Console.WriteLine("7. Display System Summary");
                Console.WriteLine("8. Exit");

                Console.Write("Enter choice: ");
                string choice = Console.ReadLine();

                try
                {
                    // TODO:
                    // Implement menu handling logic using switch-case
                    // Prompt user inputs
                    // Call appropriate UniversitySystem methods
                    switch (choice)
{
    case "1":   
        Console.Write("Enter Course Code: ");
        string courseCode = Console.ReadLine();

        Console.Write("Enter Course Name: ");
        string courseName = Console.ReadLine();

        Console.Write("Enter Credits (1-4): ");
        int credits = int.Parse(Console.ReadLine());

        Console.Write("Enter Max Capacity: ");
        int capacity = int.Parse(Console.ReadLine());

        Console.Write("Enter Prerequisites (comma-separated or press Enter): ");
        string prereqInput = Console.ReadLine();

        List<string> prerequisites = string.IsNullOrWhiteSpace(prereqInput)
            ? new List<string>()
            : prereqInput.Split(',').Select(p => p.Trim()).ToList();

        system.AddCourse(courseCode, courseName, credits, capacity, prerequisites);
        Console.WriteLine("Course added successfully.");
        break;

    case "2":   
        Console.Write("Enter Student ID: ");
        string studentId = Console.ReadLine();

        Console.Write("Enter Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Major: ");
        string major = Console.ReadLine();

        Console.Write("Enter Max Credits: ");
        int maxCredits = int.Parse(Console.ReadLine());

        Console.Write("Enter Completed Courses (comma-separated or press Enter): ");
        string completedInput = Console.ReadLine();

        List<string> completedCourses = string.IsNullOrWhiteSpace(completedInput)
            ? new List<string>()
            : completedInput.Split(',').Select(c => c.Trim()).ToList();

        system.AddStudent(studentId, name, major, maxCredits, completedCourses);
        Console.WriteLine("Student added successfully.");
        break;

    case "3":  
        Console.Write("Enter Student ID: ");
        studentId = Console.ReadLine();

        Console.Write("Enter Course Code: ");
        courseCode = Console.ReadLine();

        if (system.RegisterStudentForCourse(studentId, courseCode))
            Console.WriteLine("Registration successful.");
        else
            Console.WriteLine("Registration failed.");
        break;

    case "4":  
        Console.Write("Enter Student ID: ");
        studentId = Console.ReadLine();

        Console.Write("Enter Course Code: ");
        courseCode = Console.ReadLine();

        if (system.DropStudentFromCourse(studentId, courseCode))
            Console.WriteLine("Course dropped successfully.");
        else
            Console.WriteLine("Drop failed.");
        break;

    case "5": 
        system.DisplayAllCourses();
        break;

    case "6":  
        Console.Write("Enter Student ID: ");
        studentId = Console.ReadLine();
        system.DisplayStudentSchedule(studentId);
        break;

    case "7":  
        system.DisplaySystemSummary();
        break;

    case "8":  
        exit = true;
        Console.WriteLine("Exiting system...");
        break;

    default:
        Console.WriteLine("Invalid choice. Please try again.");
        break;
}
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}

