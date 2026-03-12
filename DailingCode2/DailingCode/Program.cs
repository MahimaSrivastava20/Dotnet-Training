using System;
using System.Collections.Generic;
using DialingCodesApp;

namespace DialingCodesApp
{
    class Program
    {
        static void Main()
        {
            Dictionary<int, string> dict;

            Console.WriteLine("Task 1 is below:");
            dict = DialingCodes.GetEmptyDictionary();
            PrintDictionary(dict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 2 is below:");
            dict = DialingCodes.GetExistingDictionary();
            PrintDictionary(dict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 3 is below:");
            Dictionary<int, string> japanDict =
                DialingCodes.AddCountryToEmptyDictionary(81, "Japan");
            PrintDictionary(japanDict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 4 is below:");
            dict = DialingCodes.AddCountryToExistingDictionary(dict, 44, "United Kingdom");
            PrintDictionary(dict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 5 is below:");
            Console.WriteLine(
                DialingCodes.GetCountryNameFromDictionary(dict, 91));
            Console.WriteLine("------------------------------------------");    

            Console.WriteLine("\nTask 6 is below:");
            Console.WriteLine(
                DialingCodes.CheckCodeExists(dict, 1));
            Console.WriteLine("------------------------------------------");    

            Console.WriteLine("\nTask 7 is below:");
            dict = DialingCodes.UpdateDictionary(dict, 91, "Republic of India");
            PrintDictionary(dict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 8 is below:");
            dict = DialingCodes.RemoveCountryFromDictionary(dict, 55);
            PrintDictionary(dict);
            Console.WriteLine("------------------------------------------");

            Console.WriteLine("\nTask 9 is below:");
            Console.WriteLine(DialingCodes.FindLongestCountryName(dict));
            Console.WriteLine("------------------------------------------");
        }

        static void PrintDictionary(Dictionary<int, string> dict)
        {
            if (dict.Count == 0)
            {
                Console.WriteLine("Dictionary is empty");
                return;
            }

            foreach (KeyValuePair<int, string> item in dict)
            {
                Console.WriteLine(item.Key+" : "+item.Value);
            }
        }
    }
}
