using System;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;

class Program
{
    static void Main()
    {
        Console.WriteLine("===== NORMAL PARAMETER =====");
        int a = 10;
        NormalMethod(a);
        Console.WriteLine($"After NormalMethod: {a}");
        Console.WriteLine();

        Console.WriteLine("===== REF PARAMETER =====");
        int b = 10;
        RefMethod(ref b);
        Console.WriteLine($"After RefMethod: {b}");
        Console.WriteLine();

        Console.WriteLine("===== REF READONLY PARAMETER =====");
        int c = 10;
        RefReadonlyMethod(ref c);
        Console.WriteLine($"After RefReadonlyMethod: {c}");
        Console.ReadLine();
    }


    static void NormalMethod(int number)
    {
        number = number + 5;
        Console.WriteLine($"Inside NormalMethod: {number}");
    }

    // 2️⃣ ref parameter (can modify original)
    static void RefMethod(ref int number)
    {
        number = number + 5;
        Console.WriteLine($"Inside RefMethod: {number}");
    }

    // 3️⃣ ref readonly parameter (cannot modify original)
    static void RefReadonlyMethod(ref readonly int number)
    {

        Console.WriteLine($"Inside RefReadonlyMethod: {number}");

        // number = number + 5; // ❌ Compile-time error
    }
}