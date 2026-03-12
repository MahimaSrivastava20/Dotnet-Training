using System;
using System.Diagnostics;
using System.Linq;
class Program
{
    public static void Main()
    {
        Student student = new Student()
        {
            name="Mahima",
            //grade="A",
            marks=97
        };
        List<Student> list = new List<Student>();
            list.Add(student);
            list.Add(new Student{name = "Mahima ", marks =87 });
            list.Add(new Student{name = "Mansi ", marks =88 });
            list.Add(new Student{name = "Devashish ", marks =89 });
        
            var result = list.Select( s=> new
            {
                s.name, grade = s.marks > 60 ? "PASS" : "FAIL"
            });
            Console.WriteLine(result.GetType());



            var sortedBySalary = list.OrderBy(e => e.marks);
            foreach(var obj in list)
            {
                Console.WriteLine($"User Name {obj.name},User Marks: {obj.marks}");
            }
            Console.WriteLine("========================");

            var sortedBySalaryName = list.OrderBy(e => e.marks).ThenBy(e => e.name);

            foreach (var obj in list)
            {
                Console.WriteLine($"User Name: {obj.name}, User Marks: {obj.marks}");
            }
                Console.WriteLine("-----------------------------------");

            List<int> numbers = new List<int>{12, 23, 34, 45};
            int first = numbers.First();
            Console.WriteLine("First Number: " + first);

            int resul = numbers.First(n => n > 25);
            Console.WriteLine("First Number Greater Than 25: " + resul);





        

        /*


        Console.WriteLine("Hello, World!");
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.WriteLine("Application Execution started.");

        int a = 10;
        int b = 0;

        try
        {
            int res = a / b;
            Console.WriteLine(res);
        }catch(Exception ex)
        {
            Trace.WriteLine("Execution occured: " + ex.Message);
        }
        Trace.WriteLine("Application Ended. ");

        Console.WriteLine("-----------------------------------");

        // Calculator.CalculatorExecution();

        Console.WriteLine("-----------------------------------");

        int total = 0;

        for(int i=0; i<=5; i++)
        {
            total += i;
        }
        Console.WriteLine(total);

        List<User> users = new List<User>();

        users.Add(new User{Name = "Aryan", Age = 22});
        users.Add(new User{Name = "Mohit", Age = 32});
        users.Add(new User{Name = "Sushant", Age = 68});
        users.Add(new User{Name = "Ritik", Age = 63});
        users.Add(new User{Name = "Sahil", Age = 52});

        foreach(var user in users)
        {
            Console.WriteLine($"User Name: {user.Name}, User Age: {user.Age}");
        }

        Queue<int> queue = new Queue<int>();
        queue.Enqueue(45);
        queue.Enqueue(55);
        queue.Enqueue(65);
        queue.Enqueue(75);
        queue.Enqueue(25);

        while(queue.Count > 0)
        {
            Console.Write(queue.Dequeue() + " ");
        }


    }

    

    class User
    {
        public string Name {get; set;}
        public int Age {get; set;}


        
    // }*/
}
}