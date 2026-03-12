using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnetoManyMVCDB.Data;

namespace OnetoManyMVCDB.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // Question 1
        // Customers whose total orders > 1000

        public IActionResult OrdersAbove1000()
        {
            var result = _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.Orders.Sum(o => o.Amount) > 1000)
                .ToList();

            return View(result);
        }

        // Question 2
        // Who purchased iPhone

        public IActionResult WhoPurchasedIphone()
        {
            var result = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.ProductName == "iPhone")
                .ToList();

            return View(result);
        }

        // Question 3
        // Show all customers with orders

        public IActionResult CustomerOrders()
        {
            var result = _context.Customers
                .Include(c => c.Orders)
                .ToList();

            return View(result);
        }
    }
}
