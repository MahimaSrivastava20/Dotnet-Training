using System;
using EcommerceAssessment;
namespace DeligateProject
{
    public class OrderProcessor
    {
        public event Action<string> OrderProcessed;

        public void ProcessOrder(
            Order order,
            Func<double, double> taxCalculator,
            Func<double, double> discountCalculator,
            Predicate<Order> validator,
            OrderCallback callback)   
        {
            if (!validator(order))
            {
                callback("Callback: Order validation failed.");
                return;
            }

            double tax = taxCalculator(order.Amount);
            double discount = discountCalculator(order.Amount);

            order.Amount = order.Amount + tax - discount;

            callback("Callback: Order " + order.OrderId + " processed successfully.");

            if (OrderProcessed != null)
            {
                OrderProcessed("Event: Order " + order.OrderId + " completed.");
            }
        }
    }
}
