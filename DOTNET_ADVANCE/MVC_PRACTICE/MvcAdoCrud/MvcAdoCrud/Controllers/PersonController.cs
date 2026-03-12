using Microsoft.AspNetCore.Mvc;
using MvcAdoCrud.Data;
using MvcAdoCrud.Models;

namespace MvcAdoCrud.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonDB db;

        public PersonController(PersonDB _db)
        {
            db = _db;
        }

        // READ
        public IActionResult Index()
        {
            return View(db.GetAll());
        }

        // CREATE
        [HttpPost]
        public IActionResult Create(Person p)
        {
            if (!ModelState.IsValid)
                return View("Index", db.GetAll());   // show validation errors

            db.Insert(p);
            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            db.Delete(id);
            return RedirectToAction("Index");
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            return View(db.GetById(id));
        }

        // EDIT POST
        [HttpPost]
        public IActionResult Edit(Person p)
        {
            db.Update(p);
            return RedirectToAction("Index");
        }
    }
}