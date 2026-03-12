using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentsMarksAPI.Models;

namespace StudentsMarksAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly CollegeDbContext _context;

        public StudentController(CollegeDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = _context.Students.Select(s => new
            {
                s.Id,
                s.Name,
                s.M1,
                s.M2,
                Total = (s.M1 ?? 0) + (s.M2 ?? 0),
                Grade = CalculateGrade((s.M1 ?? 0) + (s.M2 ?? 0))
            }).ToList();

            return Ok(students);
        }
        private static string CalculateGrade(int total)
        {
            if (total >= 180) return "A";
            if (total >= 150) return "B";
            if (total >= 100) return "C";
            return "Fail";
        }
    }
    }
