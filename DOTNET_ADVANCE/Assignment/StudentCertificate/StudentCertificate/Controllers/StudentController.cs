using Microsoft.AspNetCore.Mvc;
using StudentCertificate.DTO;
using StudentCertificate.Model;

namespace StudentCertificate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private static List<Student> students = new List<Student>();
        private static int nextId = 1;


        // CREATE STUDENT
        [HttpPost]
        public IActionResult CreateStudent(StudentCreateRequestDTO dto)
        {
            var student = new Student
            {
                Id = nextId++,
                Name = dto.Name,
                Age = dto.Age
            };

            students.Add(student);

            return Ok(student);
        }


        // UPDATE MARKS
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, StudentUpdateMarksDTO dto)
        {
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            student.M1 = dto.M1;
            student.M2 = dto.M2;

            student.Total = dto.M1 + dto.M2;

            if (student.Total >= 90)
                student.Grade = "A";
            else if (student.Total >= 70)
                student.Grade = "B";
            else
                student.Grade = "C";

            return Ok(student);
        }


        // PRINT CERTIFICATE
        [HttpGet("{id}")]
        public IActionResult PrintCertificate(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound();

            var response = new StudentResponseDTO
            {
                Id = student.Id,
                Name = student.Name,
               
                M1 = student.M1,
                M2 = student.M2,
                Total = student.Total,
                Grade = student.Grade
            };

            return Ok(response);
        }
    }

}

