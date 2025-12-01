// AsyncCallbackDemo.cs
using System;
using System.Threading.Tasks;

namespace AsyncCallbackDemo
{
    // This simulates a real library (e.g. Redis, RabbitMQ, SignalR, etc.)
    public class EventSubscriber
    {
        // This API forces you to return Task — even if you have no async work!
        // “Give me a function that accepts a string and returns a Task. I will save it and call it later.”
        // Func<string, Task>“A function that takes a string and returns a Task”
        public static void RegisterSubscribeCb(Func<string, Task> callback)
        {
            Console.WriteLine("[Library] Callback registered (expects async handler)\n");

            // Simulate events coming in
            // Task.Run(                     // ← Schedule work on the ThreadPool (the lambda) - picked up by a background thread
            //     async () => { ... }       // ← This is an async lambda (returns a completed Task)
            // );
            //Task.Run(async () => // call static method Task.Run to run the callback (the lambda) on a background thread
            // creates a Task object to represent the work
            // Normally, we'd just defined Task.Run() as above, but to show the inner workings better let's capture the Task object and
            // do some extrta logging about it:
            var backgroundTask = Task.Run(async () =>
            {
                await Task.Delay(800);
                Console.WriteLine("[Library] → Event 1: \"UserLoggedIn\"");

                // This line calls the lambda (runs: Console.WriteLine("Sub to UserLoggedIn")
                // await keyword here means: "I will wait for your Task to complete before continuing" and it needs a Task-returning method
                // The lambda returns an already-completed Task (thanks to it being defind as async or having returned Task.CompletedTask)
                // → await Task.CompletedTask → instantly completes
                // → Your lambda finishes, returns a completed Task
                // → Library continues to next event
                await callback("UserLoggedIn"); // "After 800ms, I will call your callback"

                await Task.Delay(600);
                Console.WriteLine("[Library] → Event 2: \"OrderPlaced\"");
                await callback("OrderPlaced");

                await Task.Delay(500);
                Console.WriteLine("[Library] → Event 3: \"MessageReceived\"");
                await callback("MessageReceived");
            });

            // LOG EVERYTHING ABOUT THE TASK OBJECT!
            Console.WriteLine($"[Library] Task.Run just created a REAL Task object:");
            Console.WriteLine($"   → Task ID          : {backgroundTask.Id}");
            Console.WriteLine($"   → Task Status      : {backgroundTask.Status}");
            Console.WriteLine($"   → IsCompleted      : {backgroundTask.IsCompleted}");
            Console.WriteLine($"   → IsFaulted        : {backgroundTask.IsFaulted}");
            Console.WriteLine($"   → Creation Thread  : {Environment.CurrentManagedThreadId}");
            Console.WriteLine($"   → Task Type        : {backgroundTask.GetType().Name}");
            Console.WriteLine($"   → AsyncState       : {backgroundTask.AsyncState ?? "null"}");
            Console.WriteLine($"   → ReferenceEquals(Task.CompletedTask, backgroundTask) : {ReferenceEquals(Task.CompletedTask, backgroundTask)}");
            Console.WriteLine();

            // Define a lambda function to run when the background task completes
            backgroundTask.ContinueWith(t =>
            {
                Console.WriteLine("\n[Library] Background task FINISHED!");
                Console.WriteLine($"Task {t.Id} is now: {t.Status}");
                Console.WriteLine($"   → Final Status   : {t.Status}");
                Console.WriteLine($"   → IsCompleted    : {t.IsCompleted}");
                Console.WriteLine($"   → Exception      : {t.Exception?.GetBaseException().Message ?? "none"}");
            });
        }
    }

    class Program
    {
        static async Task Main()
        {
            var subscriber = new EventSubscriber();

            Console.WriteLine("=== Demo: Why we use async x => {{ ... await Task.CompletedTask }} ===\n");

            // RegisterSubscribeCb is called once and saves the lambda (but doesn't run it yet)
            // It's saved into a variable (a delegate) inside the library (EventSubscriber) object.
            // The lambda runs multiple times, once per event
            // The lambda is marked async so a state machine is generated that returns Task
            // The lambda BECOMES a real Func<string, Task> (created as a hidden method by the compiler)
            // x is the parameter (event name, a string)
            EventSubscriber.RegisterSubscribeCb(async x =>
            {
                Console.WriteLine($"   [First example] Subscribed to event: {x}");
                // Compiler turns "await Task.CompletedTask;" into "return Task.CompletedTask;"
                // it returns one single, immortal, already-finished Task object
                // Returning Task.CompletedTask is your way of saying: “I’m done instantly — you can await me safely, but there’s no wait.”
                // await Task.CompletedTask; returns void                //
                // This line is 100% optional... Go on... comment the await out and see that everything still works!
                // The compiler turns await Task.CompletedTask; into return Task.CompletedTask (which is optional due to async lambda);
                await Task.CompletedTask;  // Any async lambda or method automatically returns a Task (or Task<T>) so not strictly needed!
            });

            // so you can do this too (no await inside) since the async lambda returns Task anyway:
            EventSubscriber.RegisterSubscribeCb(async x =>
            {
                Console.WriteLine($"   [Second example] Subscribed to event: {x}");
                // Since you an async lambda returns a Task, you don't need to explicitly return Task.CompletedTask here.
            });

            // 2 more ways of doing this even cleaner and better...

            // no need for async/await if you do nothing async inside
            // cleaner and more efficient without async state machine overhead
            EventSubscriber.RegisterSubscribeCb(x =>
            {
                Console.WriteLine($"   [Third example] Subscribed to event: {x}");
                return Task.CompletedTask; // no async needed in the lambda signature (x => { ... })
            });

            // Or even shorter(if you do nothing):
            EventSubscriber.RegisterSubscribeCb(_ => Task.CompletedTask);


            // Keep app alive to see output
            Console.WriteLine("\nWaiting for events... (press any key to exit)");
            Console.ReadKey();
        }
    }
}
