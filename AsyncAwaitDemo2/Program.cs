using System;
using System.Threading.Tasks;

namespace AsyncAwaitTrueNonBlockingDemo
{
    public class LongRunningTask
    {
        public Task<string> PerformTaskAsync()
        {
            Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Starting background work...");

            // Fire-and-forget style: return a Task that completes later
            return Task.Delay(3000).ContinueWith(_ =>
            {
                Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Background work finished on thread {Environment.CurrentManagedThreadId}");
                return "Async task completed successfully!";
            }, TaskScheduler.Default);
        }
    }

    class Program
    {
        static async Task Main()
        {
            var task = new LongRunningTask();

            Console.WriteLine($"Main thread ID: {Environment.CurrentManagedThreadId}");
            Console.WriteLine("=== Starting demos ===\n");

            // Sync demo (unchanged)
            Console.WriteLine("--- Synchronous (blocking) ---\n");
            string syncResult = new LongRunningTask().PerformTaskSync();
            OnTaskCompleted(syncResult);

            // ASYNC DEMO — now truly non-blocking, just like the callback version!
            Console.WriteLine("\n--- Async/Await – Fire and Forget (exactly like callback) ---\n");

            // Start the task but DO NOT await it yet
            var asyncTask = task.PerformTaskAsync();

            Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - I'm completely free! Background task is running...");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - Main thread doing work {i + 1}/5");
                await Task.Delay(600); // still non-blocking
            }

            // Now we attach the continuation — this runs whenever the task finishes
            // BUT we do NOT block the rest of Main here!
            asyncTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Console.WriteLine($"[Error] Task failed: {t.Exception?.GetBaseException()}");
                else
                    OnTaskCompleted(t.Result);
            }, TaskScheduler.Default);

            // This line runs IMMEDIATELY — no waiting!
            Console.WriteLine("\n[Main] Background task is still running, but I'm showing the message NOW!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static string PerformTaskSync()
        {
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Starting sync task...");
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Sync task done.");
            return "Sync task done!";
        }

        static void OnTaskCompleted(string result)
        {
            Console.WriteLine($"[Result] {DateTime.Now:HH:mm:ss.fff} - Received on thread {Environment.CurrentManagedThreadId}: {result}");
        }
    }
}
