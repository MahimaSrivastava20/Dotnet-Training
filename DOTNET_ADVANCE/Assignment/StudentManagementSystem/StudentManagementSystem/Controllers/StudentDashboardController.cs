using Microsoft.AspNetCore.Mvc;

namespace StudentManagementSystem.Controllers
{
    public class StudentDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
