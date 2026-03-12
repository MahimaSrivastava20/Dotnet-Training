using Microsoft.AspNetCore.Mvc;
using MVCwithADO.Data;
//namespace ConnectionStringMVC.Controllers
namespace MVCwithADO.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentRepository _repo;

        public StudentController(StudentRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var students = _repo.GetAllStudents();
            return View(students);
        }
    }
}
