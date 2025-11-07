using System;
using System.Net.Http;
using System.Threading.Tasks;

// Simple class to demonstrate async programming
public class AsyncDemo
{
    // Async Main method (supported in C# 7.1+)
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting async operations...");

        // Start multiple async tasks concurrently
        Task<string> task1 = FetchDataAsync("https://example.com/api/data1");  // Simulated API call
        Task<string> task2 = FetchDataAsync("https://example.com/api/data2");

        // Await both tasks (non-blocking)
        string result1 = await task1;
        string result2 = await task2;

        Console.WriteLine("Results:");
        Console.WriteLine(result1);
        Console.WriteLine(result2);

        // Use Task.WhenAll for parallelism
        string[] results = await Task.WhenAll(
            FetchDataAsync("https://example.com/api/data3"),
            FetchDataAsync("https://example.com/api/data4")
        );

        Console.WriteLine("Parallel results:");
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine("All operations completed.");
        Console.ReadKey();
    }

    // Async method example
    private static async Task<string> FetchDataAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Simulate delay for async behavior
                await Task.Delay(2000);  // 2-second delay to mimic network call

                // Actual async HTTP call (replace with real URL for testing)
                // string response = await client.GetStringAsync(url);
                // return response;

                // Simulated response
                return $"Data from {url} fetched asynchronously at {DateTime.Now}";
            }
            catch (Exception ex)
            {
                return $"Error fetching {url}: {ex.Message}";
            }
        }
    }
}
