/*
 * sp with 1 input and 1 output parameter
 * use try/catch 
 * gender based total count from CollegeMaster
 * male->4(just an example)
 * female->3*/

//using System;
//using System.Data.SqlClient; // Use Microsoft.Data.SqlClient for modern .NET
//using System.Data;
//using System.Reflection.PortableExecutable;

//class Program
//{
//    static void Main()
//    {
//        // 1. Define the connection string
//        // Replace [ServerName] and [DatabaseName] with your actual SQL Server details.
//        // Use "Trusted_Connection=True" for Windows Authentication (integrated security)
//        // or "User Id=myUsername;Password=myPassword;" for SQL Server Authentication.
//        string connectionString = "Data Source=VAANI\\SQLEXPRESS_2025;" +
//            "initial catalog=CollegeMaster1;Integrated Security=True;Connect Timeout=30;" +
//            "Encrypt=True;TrustServerCertificate=True;";

//        // 2. Create a SqlConnection object within a 'using' statement
//        // The 'using' statement ensures the connection is automatically closed and disposed
//        // even if errors occur.
//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                string gender = "female";
//                //string department = "btech";
//                // 3. Open the connection
//                connection.Open();
//                Console.WriteLine("Connection successful!");

//                // 4. Define and execute a SQL command
//                //string query = "SELECT name,department from CollegeMaster where gender = @gender";

//                using (SqlCommand command = new SqlCommand("usp_GetGenderCount ", connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.Parameters.AddWithValue("@gender", "male");



//                    SqlParameter outputParam =new SqlParameter("@totalCount", SqlDbType.Int);
//                    outputParam.Direction = ParameterDirection.Output;
//                    command.Parameters.Add(outputParam);

//                    // Execute for MALE
//                    command.ExecuteNonQuery();
//                    Console.WriteLine(
//                        "Male Count: " +
//                        command.Parameters["@totalCount"].Value);

//                    // Change gender to FEMALE (reuse same command)
//                    command.Parameters["@gender"].Value = "female";

//                    command.ExecuteNonQuery();
//                    Console.WriteLine(
//                        "Female Count: " +
//                        command.Parameters["@totalCount"].Value);
//                    //command.Parameters.AddWithValue("@department", department);
//                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
//                    // Use parameters to prevent SQL injection
//                    //SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);







//                    // Use SqlDataReader to read data from the database
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        Console.WriteLine("\nReading data...");
//                        while (reader.Read())
//                        {
//                            // Access data by column name or index
//                            Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]}");
//                        }
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                // Handle any errors that may occur during the connection or query
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//            // The connection is implicitly closed when the 'using' block ends (even in case of error)
//            Console.WriteLine("Connection closed.");
//            Hyy();
//        }
//    }

//    private static void Hyy()
//    {
//    }
//}



























































//--------------------------------------------------------------------------------------------------
//  ---------------------------------- F   I   R   S   T--------------------------------------------
//--------------------------------------------------------------------------------------------------


//using System;
//using System.Data.SqlClient; // Use Microsoft.Data.SqlClient for modern .NET
//using System.Data;
//using System.Reflection.PortableExecutable;

//class Program
//{
//    static void Main()
//    {
//        // 1. Define the connection string
//        // Replace [ServerName] and [DatabaseName] with your actual SQL Server details.
//        // Use "Trusted_Connection=True" for Windows Authentication (integrated security)
//        // or "User Id=myUsername;Password=myPassword;" for SQL Server Authentication.
//        string connectionString = "Data Source=VAANI\\SQLEXPRESS_2025;" +
//            "initial catalog=CollegeMaster1;Integrated Security=True;Connect Timeout=30;" +
//            "Encrypt=True;TrustServerCertificate=True;";

//        // 2. Create a SqlConnection object within a 'using' statement
//        // The 'using' statement ensures the connection is automatically closed and disposed
//        // even if errors occur.
//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                string gender = "female";
//                string department = "btech";
//                // 3. Open the connection
//                connection.Open();
//                Console.WriteLine("Connection successful!");

//                // 4. Define and execute a SQL command
//                //string query = "SELECT name,department from CollegeMaster where gender = @gender";

//                using (SqlCommand command = new SqlCommand("usp_GetStudentRecordWithParameter ", connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.Parameters.AddWithValue("@gender", gender);
//                    command.Parameters.AddWithValue("@department", department);
//                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
//                    // Use parameters to prevent SQL injection
//                    //SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);





//                    // Use SqlDataReader to read data from the database
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        Console.WriteLine("\nReading data...");
//                        while (reader.Read())
//                        {
//                            // Access data by column name or index
//                            Console.WriteLine($"{reader[0]} {reader[1]} {reader[2]} {reader[3]}");
//                        }
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                // Handle any errors that may occur during the connection or query
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//            // The connection is implicitly closed when the 'using' block ends (even in case of error)
//            Console.WriteLine("Connection closed.");
//            Hyy();
//        }
//    }

//    private static void Hyy()
//    {
//    }
//}





//------------------------------------------------  D   A   Y   -   3  ---------------------------------------------
// get the total count of hostel students(use execute scaler for this)
// if(hostel students >5) delete one or more records based on any category(use non query)
// else show all the records(use execute reader).

//using System;
//using System.Data;
//using System.Data.SqlClient;

//class Program
//{
//    static void Main()
//    {
//        string connectionString =
//            "Data Source=VAANI\\SQLEXPRESS_2025;" +
//            "Initial Catalog=CollegeMaster1;" +
//            "Integrated Security=True;" +
//            "Encrypt=True;TrustServerCertificate=True;";

//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                connection.Open();
//                Console.WriteLine("Connection successful!");

//                SqlCommand Command =
//                    new SqlCommand("SELECT COUNT(*) FROM BoyHostel", connection);

//                int totalCount = (int)Command.ExecuteScalar();

//                if (totalCount > 5)
//                {
//                    SqlCommand deleteCommand = new SqlCommand(
//                        @"DELETE FROM BoyHostel
//                          WHERE id IN (
//                              SELECT TOP (@extra) id
//                              FROM BoyHostel
//                              ORDER BY id DESC
//                          )", connection);

//                    deleteCommand.Parameters.AddWithValue("@extra", totalCount - 5);
//                    deleteCommand.ExecuteNonQuery();
//                    Console.WriteLine($"There were {totalCount} records and now the extra records are deleted");
//                }
//                else
//                {
//                    SqlCommand readCommand =
//                        new SqlCommand("SELECT * FROM BoyHostel", connection);

//                    using (SqlDataReader reader = readCommand.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Console.WriteLine(
//                                $"{reader[0]} {reader[1]} {reader[2]}");
//                        }
//                    }
//                }
//            }
//            catch (SqlException ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//            Console.WriteLine("Connection closed.");
//            Hyy();
//        }
//    }

//    private static void Hyy()
//    {
//    }
//}

//----------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------



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
                // 3. Open the connection
                connection.Open();
                Console.WriteLine("Connection successful!");

                // 4. Define and execute a SQL command
                string query = "SELECT dbo.FnSquare(@num)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    //// Use parameters to prevent SQL injection
                    //SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@num", 5);
                    int square = Convert.ToInt32(command.ExecuteScalar());
                    Console.WriteLine($"Bonus Amount = {square}");







                    // Use SqlDataReader to read data from the database
                    //using (SqlDataReader reader = command.ExecuteReader())
                    //{
                    //    Console.WriteLine("\nReading data...");
                    //    while (reader.Read())
                    //    {
                    //        // Access data by column name or index
                    //        //Console.WriteLine($"{reader["Name"]}");
                    //    }
                    //}
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
