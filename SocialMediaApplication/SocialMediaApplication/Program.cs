using System;

namespace MiniSocialMedia
{
    class Program
    {
        static void Main()
        {
            try
            {
                Repository<User> userRepo = new Repository<User>();

                

                User user1 = new User("Aman", "aman@mail.com");
                User user2 = new User("Neha", "neha@mail.com");

                userRepo.Add(user1);
                userRepo.Add(user2);

                user1.OnNewPost += post =>
                {
                    Console.WriteLine("New post created:");
                    Console.WriteLine(post);
                };

                user1.Follow("Neha");

                user1.AddPost("Learning C# is fun #dotnet #coding");
                user2.AddPost("Good morning everyone");

                Console.WriteLine();
                Console.WriteLine("All Users:");
                foreach (var user in userRepo.GetAll())
                {
                    Console.WriteLine(user.GetDisplayName());
                }

                Console.WriteLine();
                Console.WriteLine("Aman's Posts:");
                foreach (var post in user1.GetPosts())
                {
                    Console.WriteLine(post);
                    Console.WriteLine();
                }
            }
            catch (SocialException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }
    }
}