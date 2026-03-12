using WebApplication2.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        public IActionResult Index()
        {
            var students = _studentService.GetStudent();
            return View(students);

        }
        public IActionResult Create()
        {
            return View();
        }

        //create post
        [HttpPost]
        public IActionResult Create(WebApplication2.Models.student s)
        {
            _studentService.AddStudent(s);
            return RedirectToAction("Index");
        }

        //edit get
        


    }
}
