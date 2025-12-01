using System;
using System.Threading.Tasks;

namespace AsyncAwaitDemo3
{
    public class LongRunningTask
    {
        public Task<string> PerformTaskAsync()
        {
            Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Starting background work...");

            return Task.Delay(3000).ContinueWith(_ =>
            {
                Console.WriteLine($"[Async] {DateTime.Now:HH:mm:ss.fff} - Background work done (thread {Environment.CurrentManagedThreadId})");
                return "Async task completed successfully!";
            }, TaskScheduler.Default);
        }
    }

    class Program
    {
        static async Task Main()
        {
            var worker = new LongRunningTask();

            Console.WriteLine($"Main thread ID: {Environment.CurrentManagedThreadId}");
            Console.WriteLine("=== Async Fire-and-Forget Demo ===\n");

            // Start the task and deliberately ignore the result (fire-and-forget)
            _ = worker.PerformTaskAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine($"[Error] Task failed: {task.Exception?.GetBaseException()}");
                }
                else if (task.IsCompletedSuccessfully)
                {
                    OnTaskCompleted(task.Result);
                }
            }, TaskScheduler.Default);

            Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - Task started. I'm free to continue immediately!");

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - Doing other work {i + 1}/5");
                await Task.Delay(100);
            }

            Console.WriteLine("\n[Main] All my work is done. Showing exit message NOW!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(); // Appears immediately — just like the callback version!
        }

        static void OnTaskCompleted(string result)
        {
            Console.WriteLine($"[Result] {DateTime.Now:HH:mm:ss.fff} - Received on thread {Environment.CurrentManagedThreadId}: {result}");
        }
    }
}
