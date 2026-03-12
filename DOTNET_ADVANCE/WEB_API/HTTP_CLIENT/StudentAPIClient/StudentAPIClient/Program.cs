using System.Net.Http;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        HttpClient client = new HttpClient();

        string url = "http://localhost:5252/api/students";

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();

            var students = JsonSerializer.Deserialize<List<string>>(json);

            foreach (var student in students)
            {
                Console.WriteLine(student);
            }
        }
        else
        {
            Console.WriteLine("API call failed");
        }
    }
}
