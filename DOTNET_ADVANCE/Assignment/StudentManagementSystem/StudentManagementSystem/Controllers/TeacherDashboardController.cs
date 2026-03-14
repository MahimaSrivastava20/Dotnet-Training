using Microsoft.AspNetCore.Mvc;

namespace StudentManagementSystem.Controllers
{
    public class TeacherDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
