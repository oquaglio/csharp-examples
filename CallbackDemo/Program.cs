using System;
using System.Threading;

namespace CallbackDemo
{
    // Define a delegate type for the callback. This represents a method that takes a string result and returns void.
    public delegate void CallbackHandler(string result);

    // A class that simulates a long-running task and uses a callback to notify when it's done.
    public class LongRunningTask
    {
        // Method that performs the task and invokes the callback when complete.
        public void PerformTask(CallbackHandler callback)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Starting long-running task...");

            // Simulate work with a delay
            Thread.Sleep(2000); // 2 seconds delay

            string result = "Task completed successfully!";
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Task finished. Invoking callback...");

            // Invoke the callback if it's not null (The?. is null - conditional(safe if callback is null).
            // This calls the method referenced by the delegate, passing the result string.
            callback?.Invoke(result);
        }
    }

    class Program
    {
        // The callback method that will be called when the task is done.
        static void OnTaskCompleted(string result)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Callback received: {result}");
        }

        static void Main(string[] args)
        {
            // Create an instance of the task class
            LongRunningTask task = new LongRunningTask();

            // Create a delegate instance pointing to the callback method
            CallbackHandler callback = new CallbackHandler(OnTaskCompleted);

            // Start the task and pass the callback
            task.PerformTask(callback);

            // Keep the console open to see the output
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - Press any key to exit...");
            Console.ReadKey();
        }
    }
}
