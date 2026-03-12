// See https://aka.ms/new-console-template for more information
//onsole.WriteLine("Hello, World!");
using BLL;
class Program
{
    public static void Main()
    {
        Class1 c1 = new Class1();
        List<string> l1 = c1.Reversedstring();
        foreach (string s in l1)
        {
            Console.WriteLine(s);
        }

    }
}

