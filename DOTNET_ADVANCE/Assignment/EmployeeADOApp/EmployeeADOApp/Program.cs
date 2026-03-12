using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string conStr = @"Server=VAANI\SQLEXPRESS_2025;Database=CompanyDB;Trusted_Connection=True;";
        SqlConnection con = new SqlConnection(conStr);

        Console.Write("Enter Department: ");
        string dept = Console.ReadLine();

        con.Open();

        //   Employees by Department
        Console.WriteLine("\nEmployees in Department:");

        SqlCommand cmd1 = new SqlCommand("sp_GetEmployeesByDepartment", con);
        cmd1.CommandType = CommandType.StoredProcedure;
        cmd1.Parameters.AddWithValue("@Department", dept);

        SqlDataReader dr = cmd1.ExecuteReader();
        while (dr.Read())
        {
            Console.WriteLine($"{dr["EmpId"]} {dr["Name"]} {dr["Phone"]} {dr["Email"]}");
        }
        dr.Close();


        //  Department Count
        SqlCommand cmd2 = new SqlCommand("sp_GetDepartmentEmployeeCount", con);
        cmd2.CommandType = CommandType.StoredProcedure;
        cmd2.Parameters.AddWithValue("@Department", dept);

        SqlParameter outParam = new SqlParameter("@TotalEmployees", SqlDbType.Int);
        outParam.Direction = ParameterDirection.Output;
        cmd2.Parameters.Add(outParam);

        cmd2.ExecuteNonQuery();
        Console.WriteLine($"\nTotal employees in {dept}: {outParam.Value}");


        //  Employee Orders
        Console.WriteLine("\nEmployee Orders Report:");

        SqlCommand cmd3 = new SqlCommand("sp_GetEmployeeOrders", con);
        cmd3.CommandType = CommandType.StoredProcedure;

        SqlDataReader dr2 = cmd3.ExecuteReader();
        while (dr2.Read())
        {
            Console.WriteLine($"{dr2["Name"]} {dr2["Department"]} {dr2["OrderId"]} {dr2["OrderAmount"]} {dr2["OrderDate"]}");
        }
        dr2.Close();


        //  Duplicate Employees
        Console.WriteLine("\nDuplicate Employees:");

        SqlCommand cmd4 = new SqlCommand("sp_GetDuplicateEmployees", con);
        cmd4.CommandType = CommandType.StoredProcedure;

        SqlDataReader dr3 = cmd4.ExecuteReader();
        while (dr3.Read())
        {
            Console.WriteLine($"{dr3["EmpId"]} {dr3["Name"]} {dr3["Phone"]} {dr3["Email"]}");
        }
        dr3.Close();

        con.Close();
        Console.WriteLine("\nDone. Press any key...");
        Console.ReadKey();
    }
}