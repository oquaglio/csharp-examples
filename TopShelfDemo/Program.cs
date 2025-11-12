using System;
using System.Threading;
using Topshelf;

namespace BasicTopShelfService
{
    // Controller for lifecycle management
    public class ServiceController
    {
        private readonly Action _cleanupAction;
        private bool _isRunning;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public ServiceController(Action cleanupAction)
        {
            _cleanupAction = cleanupAction ?? throw new ArgumentNullException(nameof(cleanupAction));
            _isRunning = false;
        }

        public void Start()
        {
            _isRunning = true;
            // Start any background tasks or loops here if needed
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel(); // Signal cancellation for any waiting tasks
            _cleanupAction?.Invoke();
        }

        public bool ShouldRun()
        {
            return _isRunning && !_cts.IsCancellationRequested;
        }

        public CancellationToken CancellationToken => _cts.Token;
    }

    // The main service class
    public class MyBasicService
    {
        private readonly ServiceController _controller;
        private Thread _workerThread;

        public MyBasicService(ServiceController controller)
        {
            _controller = controller;
        }

        public void Start()
        {
            _controller.Start();
            _workerThread = new Thread(DoWork);
            _workerThread.Start();
            Console.WriteLine("Service started.");
        }

        public void Stop()
        {
            _controller.Stop();
            _workerThread?.Join(); // Wait for worker to finish
            Console.WriteLine("Service stopped.");
        }

        private void DoWork()
        {
            while (_controller.ShouldRun())
            {
                // Simulate work (e.g., process tasks, check queues, etc.)
                Console.WriteLine("Service is running...");

                try
                {
                    // Wait for 5 seconds or until cancellation
                    Thread.Sleep(5000);
                }
                catch (ThreadInterruptedException) { }
            }
            Console.WriteLine("Worker thread exiting.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Define a simple cleanup action for the controller
            Action cleanup = () => Console.WriteLine("Performing cleanup... (e.g., close connections, save state)");

            // Create the controller
            var controller = new ServiceController(cleanup);

            // Configure and run the TopShelf host
            HostFactory.Run(x =>
            {
                x.Service<MyBasicService>(s =>
                {
                    s.ConstructUsing(name => new MyBasicService(controller));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem(); // Or configure as needed (e.g., RunAsNetworkService)
                x.SetServiceName("BasicTopShelfService");
                x.SetDisplayName("Basic TopShelf Service");
                x.SetDescription("A basic Windows service demo using TopShelf with lifecycle controller.");

                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1); // Restart after 1 minute on failure
                });

                x.StartAutomatically(); // Start mode
            });
        }
    }
}
