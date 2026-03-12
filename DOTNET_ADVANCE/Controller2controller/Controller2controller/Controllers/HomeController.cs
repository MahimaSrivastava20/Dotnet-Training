using Controller2controller.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Controller2controller.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public  IActionResult Send()
        {
            TempData["Message"] = "Hello from HomeController!";
            return RedirectToAction("Receive", "Student");
        }

        public IActionResult MyStudent()
        {
            ViewBag.Name = "Mahima";
            ViewBag.College = "lpu";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
