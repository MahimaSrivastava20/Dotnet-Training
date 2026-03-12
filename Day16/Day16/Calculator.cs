using System.Diagnostics;
class Calculator
{
    public static void CalculatorExecution()
    {
        // Trace.Listeners.Add(new ConsoleTraceListener());

        Trace.WriteLine("Program started");

        PerformCalculation(10, 5);
        PerformCalculation(10, 0);   // Error case

        Trace.WriteLine("Program ended");
    }
    static void PerformCalculation(int a, int b)
    {
        Trace.WriteLine($"Entering PerformCalculation | a={a}, b={b}");

        if (b == 0)
        {
            Trace.WriteLine("Error: Division by zero detected");
            return;
        }

        int result = Divide(a, b);

        Trace.WriteLine($"Calculation successful | Result={result}");
        Trace.WriteLine("Exiting PerformCalculation");
    }

    static int Divide(int x, int y)
    {
        Trace.WriteLine($"Dividing values | x={x}, y={y}");
        return x / y;
    }
}