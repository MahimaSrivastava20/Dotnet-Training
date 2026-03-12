using System;
using System.Collections.Generic;

public delegate bool IsEligibleforScholarship(Student std);

public class Student
{
    public int RollNo { get; set; }
    public string Name { get; set; }
    public int Marks { get; set; }
    public char SportsGrade { get; set; }

    public static string GetEligibleStudents(
        List<Student> studentsList,
        IsEligibleforScholarship isEligible)
    {
        string result = "";

        foreach (Student s in studentsList)
        {
            if (isEligible(s))
            {
                if (result.Length == 0)
                {
                    result = s.Name;
                }
                else
                {
                    result = result + ", " + s.Name;
                }
            }
        }

        return result;
    }
}

public class Program
{
    // Scholarship eligibility logic
    public static bool ScholarshipEligibility(Student std)
    {
        if (std.Marks > 80 && std.SportsGrade == 'A')
        {
            return true;
        }
        return false;
    }

    public static void Main()
    {
        List<Student> lstStudents = new List<Student>();

        Student obj1 = new Student { RollNo = 1, Name = "Mahima", Marks = 75, SportsGrade = 'A' };
        Student obj2 = new Student { RollNo = 2, Name = "Mansi", Marks = 82, SportsGrade = 'A' };
        Student obj3 = new Student { RollNo = 3, Name = "Devashish", Marks = 89, SportsGrade = 'B' };
        Student obj4 = new Student { RollNo = 4, Name = "Anaya", Marks = 86, SportsGrade = 'A' };

        lstStudents.Add(obj1);
        lstStudents.Add(obj2);
        lstStudents.Add(obj3);
        lstStudents.Add(obj4);

        IsEligibleforScholarship del = ScholarshipEligibility;

        string output = Student.GetEligibleStudents(lstStudents, del);
        Console.WriteLine(output);
    }
}
