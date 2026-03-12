// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Data.SqlClient; // Use Microsoft.Data.SqlClient for modern .NET
using System.Data;
using System.Reflection.PortableExecutable;


class Program
{
    static void Main()
    {
        // 1. Define the connection string
        // Replace [ServerName] and [DatabaseName] with your actual SQL Server details.
        // Use "Trusted_Connection=True" for Windows Authentication (integrated security)
        // or "User Id=myUsername;Password=myPassword;" for SQL Server Authentication.
        string connectionString = "Data Source=VAANI\\SQLEXPRESS_2025;" +
            "initial catalog=CollegeMaster1;Integrated Security=True;Connect Timeout=30;" +
            "Encrypt=True;TrustServerCertificate=True;";

        // 2. Create a SqlConnection object within a 'using' statement
        // The 'using' statement ensures the connection is automatically closed and disposed
        // even if errors occur.
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                string gender = "male";
                // 3. Open the connection
                connection.Open();
                Console.WriteLine("Connection successful!");

                // 4. Define and execute a SQL command
                string query = "SELECT name,department from CollegeMaster where gender = @gender";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gender", gender);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    // Use parameters to prevent SQL injection
                    SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);





                    // Use SqlDataReader to read data from the database
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("\nReading data...");
                        while (reader.Read())
                        {
                            // Access data by column name or index
                            Console.WriteLine($"{reader["Name"]} {reader["Department"]}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle any errors that may occur during the connection or query
                Console.WriteLine($"Error: {ex.Message}");
            }
            // The connection is implicitly closed when the 'using' block ends (even in case of error)
            Console.WriteLine("Connection closed.");
            Hyy();
        }
    }

    private static void Hyy()
    {
    }
}