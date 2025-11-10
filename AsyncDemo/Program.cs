using System;
using System.Net.Http;
using System.Threading.Tasks;

// Simple class to demonstrate async programming
public class AsyncDemo
{
    // Async Main method (supported in C# 7.1+)
    static async Task Main(string[] args)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Starting async operations...");

        // Start multiple async tasks concurrently
        Task<string> task1 = FetchDataAsync("https://example.com/api/data1");  // Simulated API call
        Task<string> task2 = FetchDataAsync("https://example.com/api/data2");

        // Progresses immidiately to here without waiting for tasks to complete
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Async tasks started");

        // Await both tasks (non-blocking)
        string result1 = await task1; // The main thread pauses here until task1 completes
        string result2 = await task2; // The main thread pauses here until task2 completes

        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Results:");
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {result1}");
        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {result2}");

        // Start two new async operations; both run in parallel
        // Wait for both to complete (await)
        string[] results = await Task.WhenAll(
            FetchDataAsync("https://example.com/api/data3"),
            FetchDataAsync("https://example.com/api/data4")
        );

        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Parallel results:");
        foreach (var result in results)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - {result}");
        }

        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - All operations completed.");
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
                return $"Data from {url} fetched asynchronously at {DateTime.Now:HH:mm:ss.fff}";
            }
            catch (Exception ex)
            {
                return $"Error fetching {url}: {ex.Message}";
            }
        }
    }
}
