using System;

abstract class Employee
{
    public abstract decimal GetPay();
}

class HourlyEmployee : Employee
{
    decimal rate;
    decimal hours;

    public HourlyEmployee(decimal r, decimal h)
    {
        rate = r;
        hours = h;
    }

    public override decimal GetPay()
    {
        return rate * hours;
    }
}

class SalariedEmployee : Employee
{
    decimal salary;

    public SalariedEmployee(decimal s)
    {
        salary = s;
    }

    public override decimal GetPay()
    {
        return salary;
    }
}

class CommissionEmployee : Employee
{
    decimal commission;
    decimal baseSalary;

    public CommissionEmployee(decimal c, decimal b)
    {
        commission = c;
        baseSalary = b;
    }

    public override decimal GetPay()
    {
        return baseSalary + commission;
    }
}

class Program
{
    static decimal CalculateTotalPay(string[] employees)
    {
        decimal total = 0;

        foreach (string emp in employees)
        {
            string[] parts = emp.Split(' ');

            Employee employee;

            if (parts[0] == "H")
            {
                employee = new HourlyEmployee(
                    decimal.Parse(parts[1]),
                    decimal.Parse(parts[2])
                );
            }
            else if (parts[0] == "S")
            {
                employee = new SalariedEmployee(
                    decimal.Parse(parts[1])
                );
            }
            else
            {
                employee = new CommissionEmployee(
                    decimal.Parse(parts[1]),
                    decimal.Parse(parts[2])
                );
            }

            total += employee.GetPay();
        }

        return Math.Round(total, 2);
    }

    static void Main()
    {
        string[] employees =
        {
            "H 20 8",
            "S 3000",
            "C 500 2000"
        };

        decimal result = CalculateTotalPay(employees);

        Console.WriteLine("Total Pay: " + result);
    }
}
