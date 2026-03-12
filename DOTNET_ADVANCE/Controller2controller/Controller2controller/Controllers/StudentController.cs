using Microsoft.AspNetCore.Mvc;

namespace Controller2controller.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Square()
        {
            var value = TempData["Number"];

            if (value == null)
                return Content("No number received");

            int num = Convert.ToInt32(value);
            int result = num * num;

            return Content($"Square of {num} is {result}");
        }
        public IActionResult Receive()
        {
            var msg = TempData["Message"];
            return Content(msg?.ToString());
        }
        //take 2 variable one name and one college name in controller use viewdata or viewbag anything od your choice and then return the value in view
        

    }
}
