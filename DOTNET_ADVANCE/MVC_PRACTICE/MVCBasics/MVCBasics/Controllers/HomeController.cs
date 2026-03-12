using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCBasics.Models;

namespace MVCBasics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        //------https://localhost:7211/Home/Student------
        public IActionResult Student()
        {
            var s = new { name = "Ravi", Marks = 90 };
            return Json(s);
        }

        //public IActionResult Square(int? number)
        //{
        //    if (number == null)
        //        return Content("Please provide a number");
        //    int result = number.Value * number.Value;
        //    return Content("Square = " + result);
        //}


        
        public IActionResult Add(int m1,int m2,int m3)
        {
            int sum = m1 + m2 + m3;
            return View(sum);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
