// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        string keyPath = @"Software\MyTestApp";

        // 1️⃣ Create or open key
        RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);

        // 2️⃣ Write value
        key.SetValue("Username", "Student123");
        Console.WriteLine("Value written.");

        // 3️⃣ Read value
        string username = key.GetValue("Username").ToString();
        Console.WriteLine("Read from registry: " + username);

        key.Close();

        // 4️⃣ Delete the value
        Registry.CurrentUser.OpenSubKey(keyPath, true)
                            .DeleteValue("Username");

        Console.WriteLine("Value deleted.");
    }
}





