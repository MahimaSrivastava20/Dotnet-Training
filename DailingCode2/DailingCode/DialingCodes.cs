using System.Collections.Generic;

namespace DialingCodesApp
{
    public static class DialingCodes
    {
        // Task 1
        public static Dictionary<int, string> GetEmptyDictionary()
        {
            return new Dictionary<int, string>();
        }

        // Task 2
        public static Dictionary<int, string> GetExistingDictionary()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "United States of America");
            dict.Add(55, "Brazil");
            dict.Add(91, "India");
            return dict;
        }

        // Task 3
        public static Dictionary<int, string> AddCountryToEmptyDictionary(int countryCode, string countryName)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(countryCode, countryName);
            return dict;
        }

        // Task 4
        public static Dictionary<int, string> AddCountryToExistingDictionary(
            Dictionary<int, string> existingDictionary,
            int countryCode,
            string countryName)
        {
            if (existingDictionary.ContainsKey(countryCode))
            {
                existingDictionary[countryCode] = countryName;
            }
            else
            {
                existingDictionary.Add(countryCode, countryName);
            }
            return existingDictionary;
        }

        // Task 5
        public static string GetCountryNameFromDictionary(
            Dictionary<int, string> existingDictionary,
            int countryCode)
        {
            if (existingDictionary.ContainsKey(countryCode))
            {
                return existingDictionary[countryCode];
            }
            return "";
        }

        // Task 6
        public static bool CheckCodeExists(
            Dictionary<int, string> existingDictionary,
            int countryCode)
        {
            return existingDictionary.ContainsKey(countryCode);
        }

        // Task 7
        public static Dictionary<int, string> UpdateDictionary(
            Dictionary<int, string> existingDictionary,
            int countryCode,
            string countryName)
        {
            if (existingDictionary.ContainsKey(countryCode))
            {
                existingDictionary[countryCode] = countryName;
            }
            return existingDictionary;
        }

        // Task 8
        public static Dictionary<int, string> RemoveCountryFromDictionary(
            Dictionary<int, string> existingDictionary,
            int countryCode)
        {
            if (existingDictionary.ContainsKey(countryCode))
            {
                existingDictionary.Remove(countryCode);
            }
            return existingDictionary;
        }

        // Task 9
        public static string FindLongestCountryName(
            Dictionary<int, string> existingDictionary)
        {
            string longest = "";

            foreach (var item in existingDictionary)
            {
                if (item.Value.Length > longest.Length)
                {
                    longest = item.Value;
                }
            }
            return longest;
        }
    }
}
