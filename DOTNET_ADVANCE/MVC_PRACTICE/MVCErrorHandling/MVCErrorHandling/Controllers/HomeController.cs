using Microsoft.AspNetCore.Mvc;
using MVCErrorHandling.Models;
using System.Diagnostics;

namespace MVCErrorHandling.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TestError()
        {
            int x = 10;
            int y = 0;
            int result = x / y; // This will throw a DivideByZeroException
            return Content(result.ToString());
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
