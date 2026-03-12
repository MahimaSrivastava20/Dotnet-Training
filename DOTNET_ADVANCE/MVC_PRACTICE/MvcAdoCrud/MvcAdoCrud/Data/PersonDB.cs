using System.Data.SqlClient;
using System.Data;
using MvcAdoCrud.Models;

namespace MvcAdoCrud.Data
{
    public class PersonDB
    {
        private readonly string _connection;

        public PersonDB(IConfiguration configuration)
        {
            _connection = configuration.GetConnectionString("DefaultConnection");
        }

        // READ
        public List<Person> GetAll()
        {
            List<Person> list = new List<Person>();

            using (SqlConnection con = new SqlConnection(_connection))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Persons", con);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Person
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = dr["Name"].ToString()
                    });
                }
            }
            return list;
        }

        // CREATE
        public void Insert(Person p)
        {
            using SqlConnection con = new SqlConnection(_connection);
            SqlCommand cmd = new SqlCommand("INSERT INTO Persons(Name) VALUES(@Name)", con);
            cmd.Parameters.AddWithValue("@Name", p.Name);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void Delete(int id)
        {
            using SqlConnection con = new SqlConnection(_connection);
            SqlCommand cmd = new SqlCommand("DELETE FROM Persons WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // GET BY ID
        public Person GetById(int id)
        {
            Person p = new Person();
            using SqlConnection con = new SqlConnection(_connection);
            SqlCommand cmd = new SqlCommand("SELECT * FROM Persons WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                p.Id = Convert.ToInt32(dr["Id"]);
                p.Name = dr["Name"].ToString();
            }
            return p;
        }

        // UPDATE
        public void Update(Person p)
        {
            using SqlConnection con = new SqlConnection(_connection);
            SqlCommand cmd = new SqlCommand("UPDATE Persons SET Name=@Name WHERE Id=@Id", con);
            cmd.Parameters.AddWithValue("@Name", p.Name);
            cmd.Parameters.AddWithValue("@Id", p.Id);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
