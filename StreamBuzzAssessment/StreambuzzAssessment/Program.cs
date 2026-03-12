// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;

public class CreatorStats
{
    public string CreatorName { get; set; }
    public double[] WeeklyLikes { get; set; }

    public static List<CreatorStats> EngagementBoard = new List<CreatorStats>();
}

public class Program
{
    public void RegisterCreator(CreatorStats record)
    {
        CreatorStats.EngagementBoard.Add(record);
    }

    public Dictionary<string, int> GetTopPostCounts(List<CreatorStats> records, double likeThreshold)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        foreach (var creator in records)
        {
            int count = 0;

            foreach (double likes in creator.WeeklyLikes)
            {
                if (likes >= likeThreshold)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                result.Add(creator.CreatorName, count);
            }
        }

        return result;
    }

    public double CalculateAverageLikes()
    {
        double total = 0;
        int count = 0;

        foreach (var creator in CreatorStats.EngagementBoard)
        {
            foreach (double likes in creator.WeeklyLikes)
            {
                total += likes;
                count++;
            }
        }

        if (count == 0)
            return 0;

        return total / count;
    }

    public static void Main()
    {
        Program program = new Program();
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("1. Register Creator");
            Console.WriteLine("2. Show Top Posts");
            Console.WriteLine("3. Calculate Average Likes");
            Console.WriteLine("4. Exit");
            Console.WriteLine("Enter your choice:");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    CreatorStats creator = new CreatorStats();

                    Console.WriteLine("Enter Creator Name:");
                    creator.CreatorName = Console.ReadLine();

                    creator.WeeklyLikes = new double[4];

                    Console.WriteLine("Enter weekly likes (Week 1 to 4):");

                    for (int i = 0; i < 4; i++)
                    {
                        creator.WeeklyLikes[i] = Convert.ToDouble(Console.ReadLine());
                    }

                    program.RegisterCreator(creator);

                    Console.WriteLine("Creator registered successfully");
                    break;

                case 2:
                    Console.WriteLine("Enter like threshold:");
                    double threshold = Convert.ToDouble(Console.ReadLine());

                    Dictionary<string, int> topPosts =
                        program.GetTopPostCounts(CreatorStats.EngagementBoard, threshold);

                    if (topPosts.Count == 0)
                    {
                        Console.WriteLine("No top-performing posts this week");
                    }
                    else
                    {
                        foreach (var entry in topPosts)
                        {
                            Console.WriteLine(entry.Key + " - " + entry.Value);
                        }
                    }
                    break;

                case 3:
                    double average = program.CalculateAverageLikes();
                    Console.WriteLine("Overall average weekly likes: " + average);
                    break;

                case 4:
                    Console.WriteLine("Logging off - Keep Creating with StreamBuzz!");
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}
