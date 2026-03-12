using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: Create Form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Save Data
        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Employees VALUES (@Id,@Name,@Address,@Aadhaar,@DOB,@Salary)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", emp.Id);
                    cmd.Parameters.AddWithValue("@Name", emp.Name);
                    cmd.Parameters.AddWithValue("@Address", emp.Address);
                    cmd.Parameters.AddWithValue("@Aadhaar", emp.AadhaarCard);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@Salary", emp.Salary);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                return RedirectToAction("Index");
            }

            return View(emp);
        }

        // Show Employees
        public IActionResult Index()
        {
            List<Employee> list = new List<Employee>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Employees";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Employee
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Address = reader["Address"].ToString(),
                        AadhaarCard = reader["AadhaarCard"].ToString(),
                        DOB = Convert.ToDateTime(reader["DOB"]),
                        Salary = Convert.ToDecimal(reader["Salary"])
                    });
                }
                con.Close();
            }

            return View(list);
        }
    }
}