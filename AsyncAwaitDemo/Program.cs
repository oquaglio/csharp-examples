using System;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    public class LongRunningTask
    {
        // Synchronous version - BLOCKS the calling thread
        public string PerformTaskSync()
        {
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Starting task (blocking thread)...");
            Thread.Sleep(3000);
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Task completed.");
            return "Sync task done!";
        }

        // ASYNC/AWAIT version - truly non-blocking!
        public async Task<string> PerformTaskAsync()
        {
            Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Starting async task (offloaded to thread pool)...");

            // This runs on a background thread (Task.Run is optional for CPU work, but makes intent clear)
            await Task.Delay(3000); // Non-blocking delay (better than Thread.Sleep in async code)

            // Or simulate real async I/O (this is naturally async):
            // await Task.Delay(3000); // e.g. HttpClient.GetAsync(), DB query, file read, etc.

            Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Work finished on thread {Environment.CurrentManagedThreadId}");
            return "Async task completed successfully!";
        }
    }

    class Program
    {
        static async Task Main(string[] args)  // Note: async Main (C# 7.1+)
        {
            var task = new LongRunningTask();

            Console.WriteLine($"Main thread ID: {Environment.CurrentManagedThreadId}");
            Console.WriteLine("=== Starting demos ===\n");

            // DEMO 1: Synchronous - blocks everything
            Console.WriteLine("--- Synchronous (blocking) ---\n");
            string syncResult = task.PerformTaskSync();
            OnTaskCompleted(syncResult);

            // DEMO 2: Modern async/await - main thread stays responsive!
            Console.WriteLine("\n--- Async/Await (non-blocking) ---\n");

            // Start the async work, but DON'T wait yet
            var asyncOperation = task.PerformTaskAsync();

            Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - I'm FREE! Doing useful work while waiting...");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - Main thread working... {i + 1}/5");
                await Task.Delay(600); // Non-blocking sleep!
            }

            // Now await the result - this resumes when the task finishes
            string asyncResult = await asyncOperation;
            OnTaskCompleted(asyncResult);

            Console.WriteLine("\nAll done! Press any key to exit...");
            Console.ReadKey();
        }

        static void OnTaskCompleted(string result)
        {
            Console.WriteLine($"[Result] {DateTime.Now:HH:mm:ss.fff} - Received on thread {Environment.CurrentManagedThreadId}: {result}");
        }
    }
}
