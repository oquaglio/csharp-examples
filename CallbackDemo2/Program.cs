using System;
using System.Threading;
using System.Threading.Tasks;

namespace CallbackDemoRealWorld
{
    public delegate void CallbackHandler(string result);

    public class LongRunningTask
    {
        // Synchronous version - BLOCKS the calling thread
        public string PerformTaskSync()
        {
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Starting task (blocking thread)...");
            Thread.Sleep(3000); // Blocks for 3 seconds
            Console.WriteLine($"[Sync] {DateTime.Now:HH:mm:ss.fff} - Task completed.");
            return "Sync task done!";
        }

        // Asynchronous version using callback - DOES NOT block
        public void PerformTaskAsync(CallbackHandler callback)
        {
            Console.WriteLine($"[Async+Callback] {DateTime.Now:HH:mm:ss.fff} - Starting async task on background thread...");

            // Run the work on a thread pool thread
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(3000); // Simulate real async work (e.g. DB, HTTP, file I/O)

                string result = "Async task completed successfully!";
                Console.WriteLine($"[Async+Callback] {DateTime.Now:HH:mm:ss.fff} - Work done. Invoking callback on thread {Environment.CurrentManagedThreadId}");

                // Critical: Invoke callback - preferably back on UI thread if needed
                callback?.Invoke(result);
            });
        }
    }

    class Program
    {
        static void OnTaskCompleted(string result)
        {
            Console.WriteLine($"[Callback] {DateTime.Now:HH:mm:ss.fff} - Received result on thread {Environment.CurrentManagedThreadId}: {result}");
        }

        static void Main(string[] args)
        {
            var task = new LongRunningTask();

            Console.WriteLine($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("=== Starting demos ===\n");

            // DEMO 1: Synchronous - UI would freeze!
            Console.WriteLine("--- Synchronous (blocking) ---\n");
            string syncResult = task.PerformTaskSync();
            OnTaskCompleted(syncResult); // Called directly - but main thread was blocked!

            // DEMO 2: Asynchronous with callback - main thread stays responsive!
            Console.WriteLine("\n--- Asynchronous with Callback (non-blocking) ---\n");
            task.PerformTaskAsync(OnTaskCompleted);

            // Main thread stays FREE to do other work!
            Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - I'm FREE! Doing other things while task runs in background...");
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"[Main] {DateTime.Now:HH:mm:ss.fff} - Main thread working... {i + 1}/5");
                Thread.Sleep(600);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
