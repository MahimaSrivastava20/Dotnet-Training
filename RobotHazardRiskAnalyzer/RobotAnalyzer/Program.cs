// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;

public class RobotSafetyException : Exception
{
    public RobotSafetyException(string message) : base(message)
    {
    }
}

public class RobotHazardAuditor
{
    public double CalculateHazardRisk(double armPrecision, int workerDensity, string machineryState)
    {
        if (armPrecision < 0.0 || armPrecision > 1.0)
        {
            throw new RobotSafetyException("Error:  Arm precision must be 0.0-1.0");
        }

        if (workerDensity < 1 || workerDensity > 20)
        {
            throw new RobotSafetyException("Error: Worker density must be 1-20");
        }

        double machineRiskFactor;

        if (machineryState == "Worn")
        {
            machineRiskFactor = 1.3;
        }
        else if (machineryState == "Faulty")
        {
            machineRiskFactor = 2.0;
        }
        else if (machineryState == "Critical")
        {
            machineRiskFactor = 3.0;
        }
        else
        {
            throw new RobotSafetyException("Error: Unsupported machinery state");
        }

        double hazardRisk = ((1.0 - armPrecision) * 15.0) + (workerDensity * machineRiskFactor);

        return hazardRisk;
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            Console.Write("Enter Arm Precision (0.0 - 1.0): ");
            double armPrecision = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Worker Density (1 - 20): ");
            int workerDensity = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter Machinery State (Worn/Faulty/Critical): ");
            string machineryState = Console.ReadLine();

            RobotHazardAuditor auditor = new RobotHazardAuditor();

            double risk = auditor.CalculateHazardRisk(armPrecision, workerDensity, machineryState);

            Console.WriteLine("Robot Hazard Risk Score: " + risk);
        }
        catch (RobotSafetyException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
