// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;
using System.Linq;

class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public double Salary { get; set; }
}

class Program
{
    static void Main()
    {
        var employees = new List<Employee>
        {
            new Employee{ Id=1, Name="Arun", Department="IT", Salary=60000 },
            new Employee{ Id=2, Name="Meera", Department="HR", Salary=45000 },
            new Employee{ Id=3, Name="John", Department="IT", Salary=75000 }
        };

        var result = employees
            .Where(e => e.Salary > 50000)
            .GroupBy(e => e.Department)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var dept in result)
            Console.WriteLine($"{dept.Key} → {string.Join(", ", dept.Value.Select(e => e.Name))}");
    }
}