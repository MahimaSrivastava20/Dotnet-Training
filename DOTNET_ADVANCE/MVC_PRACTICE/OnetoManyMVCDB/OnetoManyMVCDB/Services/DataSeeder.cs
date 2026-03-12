using OnetoManyMVCDB.Models;
using OnetoManyMVCDB.Data;

namespace OnetoManyMVCDB.Services
{
    public class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Customers.Any())
            {
                var c1 = new Customer { Name = "Marimuthu", City = "Chennai" };
                var c2 = new Customer { Name = "Kannan", City = "Madurai" };
                var c3 = new Customer { Name = "Ganesh", City = "Trichy" };
                var c4 = new Customer { Name = "Suresh", City = "Salem" };

                context.Customers.AddRange(c1, c2, c3, c4);
                context.SaveChanges();

                context.Orders.AddRange(

                    new Order { ProductName = "iPhone", Amount = 90000, CustomerId = c1.Id },
                    new Order { ProductName = "Laptop", Amount = 60000, CustomerId = c1.Id },
                    new Order { ProductName = "AirPods", Amount = 15000, CustomerId = c1.Id },

                    new Order { ProductName = "Samsung", Amount = 35000, CustomerId = c2.Id },
                    new Order { ProductName = "Headphones", Amount = 2000, CustomerId = c2.Id },

                    new Order { ProductName = "Keyboard", Amount = 1200, CustomerId = c3.Id }

                );

                context.SaveChanges();
            }
        }
    }
}
