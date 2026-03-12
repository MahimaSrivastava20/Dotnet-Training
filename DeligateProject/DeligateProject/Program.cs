using System;
using System.Collections.Generic;
using DeligateProject;

namespace EcommerceAssessment
{
    class Program
    {
        static void Main()
        {
            Repository<Order> repository = new Repository<Order>();

            repository.Add(new Order { OrderId = 1, CustomerName = "Alice", Amount = 5000 });
            repository.Add(new Order { OrderId = 2, CustomerName = "Bob", Amount = 2000 });
            repository.Add(new Order { OrderId = 3, CustomerName = "Charlie", Amount = 8000 });

            Func<double, double> taxCalculator = amount => amount * 0.18;
            Func<double, double> discountCalculator = amount => amount * 0.10;
            Predicate<Order> validator = order => order.Amount >= 3000;

            OrderCallback callback = message => Console.WriteLine(message);

            Action<string> logger = msg => Console.WriteLine("Logger: " + msg);
            Action<string> notifier = msg => Console.WriteLine("Notifier: " + msg);

            OrderProcessor processor = new OrderProcessor();
            processor.OrderProcessed += logger;
            processor.OrderProcessed += notifier;

            List<Order> processedOrders = new List<Order>();

            foreach (Order order in repository.GetAll())
            {
                processor.ProcessOrder(
                    order,
                    taxCalculator,
                    discountCalculator,
                    validator,
                    callback
                );

                if (validator(order))
                {
                    processedOrders.Add(order);
                }
            }

            processedOrders.Sort(
                delegate (Order o1, Order o2)
                {
                    return o2.Amount.CompareTo(o1.Amount);
                }
            );

            Console.WriteLine("\nSorted Orders (Descending Amount):");
            foreach (Order order in processedOrders)
            {
                Console.WriteLine(order);
            }
        }
    }
}
