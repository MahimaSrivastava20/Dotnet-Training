using DTOunderstanding.DTO;
using DTOunderstanding.Model;
using Microsoft.AspNetCore.Mvc;

namespace DTOunderstanding.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    
    {
        private static List<Student> students = new List<Student>();

        // POST: api/student
        [HttpPost]
        public IActionResult CreateStudent(StudentCreateRequestDTO request)
        {
            Student student = new Student
            {
                Id = students.Count + 1,
                Name = request.Name,
                Age = request.Age,
                CourseFeePaid = request.CourseFeePaid
            };

            students.Add(student);

            return Ok(student.Id);
        }

        // GET: api/student/basic
        [HttpGet("basic")]
        public ActionResult<List<StudentResponseDTO>> GetStudents()
        {
            var result = students.Select(s => new StudentResponseDTO
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            return Ok(result);
        }

        // GET: api/student/totalfees
        [HttpGet("totalfees")]
        public ActionResult<decimal> GetTotalFees()
        {
            decimal total = students.Sum(s => s.CourseFeePaid);
            return Ok(total);
        }
    }

}

