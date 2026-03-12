// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;

public class Student
{
    public string Id{get;set;}
    public string Name{get;set;}
    public string Course{get;set;}
    public int Marks{get;set;}
}




public class StudentUtility
{
    public Dictionary<string,string> GetStudentDetails(string id)
    {
        Dictionary<string,string> result = new Dictionary<string, string>();

        foreach (var student in Program.studentDetails.Values)
        {
            if (student.Id == id)
            {
                result.Add(student.Id, student.Name + "_" + student.Course);
                return result;
            }
        }

        return result;
    }

    public Dictionary<string, Student> UpdateStudentMarks(string id, int marks)
    {
        Dictionary<string, Student> result = new Dictionary<string, Student>();

        foreach (var student in Program.studentDetails.Values)
        {
            if (student.Id == id)
            {
                student.Marks = marks;
                result.Add(student.Id, student);
                return result;
            }
        }

        return result;
    }
}

public class Program
{
    public static Dictionary<int, Student> studentDetails = new Dictionary<int, Student>();

    static void Main()
    {
        studentDetails.Add(1,new Student{Id="ST01",Name="Mahima",Course="DataScience",Marks=90});
        studentDetails.Add(2,new Student{Id="ST02",Name="Mansi",Course="Java",Marks=87});
        studentDetails.Add(3,new Student{Id="ST03",Name="Devashish",Course="Python",Marks=97});

        StudentUtility utility = new StudentUtility();

        while (true)
        {
            Console.WriteLine("1. Get Student Details");
            Console.WriteLine("2. Update Marks");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Enter your choice");

            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                Console.WriteLine("Enter the student id");
                string id = Console.ReadLine();

                var result = utility.GetStudentDetails(id);

                if (result.Count == 0)
                {
                    Console.WriteLine("Student id not found");
                }
                else
                {
                    foreach (var item in result)
                    {
                        Console.WriteLine(item.Key + "   " + item.Value);
                    }
                }
            }
            else if (choice == 2)
            {
                Console.WriteLine("Enter the student id");
                string id = Console.ReadLine();

                Console.WriteLine("Enter the marks");
                int marks = int.Parse(Console.ReadLine());

                var result = utility.UpdateStudentMarks(id, marks);

                if (result.Count == 0)
                {
                    Console.WriteLine("Student id not found");
                }
                else
                {
                    foreach (var item in result)
                    {
                        Console.WriteLine(item.Key + "   " + item.Value.Marks);
                    }
                }
            }
            else if (choice == 3)
            {
                Console.WriteLine("Thank you");
                break;
            }
        }
    }
}
