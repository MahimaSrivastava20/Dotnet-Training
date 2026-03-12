

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

//        DataSet ds = new DataSet();


//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {
//                // 3. Open the connection
//                connection.Open();
//                Console.WriteLine("Connection successful!");

//                // 4. Define and execute a SQL command
//                string query = "sp_getStudentDetails";

//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
//                    //// Use parameters to prevent SQL injection
//                    //SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);
//                    command.CommandType = CommandType.StoredProcedure;


//                    SqlDataAdapter adapter = new SqlDataAdapter(command);

//                    adapter.Fill(ds, "Counts");


//                }

//                foreach (DataRowView drv in ds.Tables["Counts"].DefaultView)
//                {
//                    Console.WriteLine($"Bonus Amount = {drv[0]}");
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



////class Program
////{
////    static void Main()
////    {
////        // 1. Define the connection string
////        // Replace [ServerName] and [DatabaseName] with your actual SQL Server details.
////        // Use "Trusted_Connection=True" for Windows Authentication (integrated security)
////        // or "User Id=myUsername;Password=myPassword;" for SQL Server Authentication.
////        string connectionString = "Data Source=VAANI\\SQLEXPRESS_2025;" +
////            "initial catalog=CollegeMaster1;Integrated Security=True;Connect Timeout=30;" +
////            "Encrypt=True;TrustServerCertificate=True;";

////        // 2. Create a SqlConnection object within a 'using' statement
////        // The 'using' statement ensures the connection is automatically closed and disposed
////        // even if errors occur.
////        using (SqlConnection connection = new SqlConnection(connectionString))
////        {
////            try
////            {
////                // 3. Open the connection
////                connection.Open();
////                Console.WriteLine("Connection successful!");

////                // 4. Define and execute a SQL command
////                string query = "SELECT dbo.FnSquare(@num)";

////                using (SqlCommand command = new SqlCommand(query, connection))
////                {
////                    //SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
////                    //// Use parameters to prevent SQL injection
////                    //SqlCommandBuilder sqlee = new SqlCommandBuilder(dataAdapter);
////                    command.CommandType = CommandType.Text;
////                    command.Parameters.AddWithValue("@num", 5);
////                    int square = Convert.ToInt32(command.ExecuteScalar());
////                    Console.WriteLine($"Bonus Amount = {square}");







//                    // Use SqlDataReader to read data from the database
//                    //using (SqlDataReader reader = command.ExecuteReader())
//                    //{
//                    //    Console.WriteLine("\nReading data...");
//                    //    while (reader.Read())
//                    //    {
//                    //        // Access data by column name or index
//                    //        //Console.WriteLine($"{reader["Name"]}");
//                    //    }
//                    //}
//                }
//            }
//            catch (SqlException ex)
//            {
//                // Handle any errors that may occur during the connection or query
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//            // The connection is implicitly closed when the 'using' block ends (even in case of error)
//            Console.WriteLine("Connection closed.");
//Hyy();
//        }
//    }

//    private static void Hyy()
//{
//}
//}




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
//               string connectionString = "Data Source=VAANI\\SQLEXPRESS_2025;" +
//                    "initial catalog=CollegeMaster1;Integrated Security=True;Connect Timeout=30;" +
//                    "Encrypt=True;TrustServerCertificate=True;";


//        // 2. Create a SqlConnection object within a 'using' statement
//        // The 'using' statement ensures the connection is automatically closed and disposed
//        // even if errors occur.

//        DataSet ds = new DataSet();
//        using (SqlConnection connection = new SqlConnection(connectionString))
//        {
//            try
//            {

//                // 3. Open the connection
//                connection.Open();
//                Console.WriteLine("Connection successful!");

//                using (SqlCommand command = new SqlCommand("sp_GetStudentDetails", connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;

//                    SqlDataAdapter adapter = new SqlDataAdapter(command);

//                    adapter.Fill(ds, "Students");

//                }

//            }
//            catch (SqlException ex)
//            {
//                // Handle any errors that may occur during the connection or query
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//            // The connection is implicitly closed when the 'using' block ends (even in case of error)
//            Console.WriteLine("Connection closed.");

//        }
//    }

//}





using System;
using System.Data.SqlClient; // Use Microsoft.Data.SqlClient for modern .NET
using System.Data;
using System.Reflection.PortableExecutable;

class Program
{
    static void Main()
    {
        DataTable dt = new DataTable("Students");
        DataSet ds = new DataSet();
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Department", typeof(string));


        dt.Rows.Add(1, "Mahima", "IT");
        dt.Rows.Add(2, "Marimuthu", "MCA");
        dt.Rows.Add(3, "Ritik", "ECE");
        dt.Rows.Add(4, "Aaryan", "Civil");

        ds.Tables.Add(dt);
        Console.WriteLine(ds.Tables.Count);


    }
    }


