using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;
using System.Linq;

namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;

        public AccountController(ApplicationDbContext context)
        {
            db = context;
        }

        // REGISTER GET
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER POST
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }


        // LOGIN GET
        public IActionResult Login()
        {
            return View();
        }


        // LOGIN POST
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = db.Users
                .FirstOrDefault(x =>
                    x.Email == model.Email &&
                    x.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid Login";
                return View();
            }

            if (user.Role == "Teacher")
            {
                return RedirectToAction("Index", "TeacherDashboard");
            }
            else
            {
                return RedirectToAction("Index", "StudentDashboard");
            }
        }
    }
}