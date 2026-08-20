using Microsoft.AspNetCore.Mvc;
using StudentReportCard.Services;
using StudentReportCard.ViewModels;

namespace StudentReportCard.Controllers
{
    [Route("student")]
    public class StudentController : Controller
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginVM model)
        {
            var student = _service.Login(model.Id, model.Password);

            if (student == null)
            {
                ViewBag.Message = "Invalid Login";
                return View();
            }

            return View("IdCard", student);
        }
    }
}