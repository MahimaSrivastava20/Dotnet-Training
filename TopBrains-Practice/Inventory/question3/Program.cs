// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Globalization;
using System.Text;

class Program
{
    static string FormatProduct(string input)
    {
        input = input.Trim();

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (i == 0 || input[i] != input[i - 1])
            {
                result.Append(input[i]);
            }
        }

        string cleaned = string.Join(" ",
            result.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));

        return CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(cleaned.ToLower());
    }

    static void Main()
    {
        string input = " llapppptop bag ";
        Console.WriteLine(FormatProduct(input));
    }
}
