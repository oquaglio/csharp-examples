// Simple enum to mimic event actions (e.g., Add, Update in AFDataPipe)
// Enums are like named constants; here defining possible actions for events
public enum PipeAction
{
    Add,    // Represents adding a new data point
    Update, // Represents updating an existing data point
    Remove  // Represents removing a data point
}

// Simple class to mimic AFValue (data point with value, status, timestamp)
// Classes define custom types with properties and methods
public class PipeValue
{
    // Properties: Like variables but with getters/setters for controlled access
    public object Value { get; set; }  // Can hold any type (object is generic base type)
    public string Status { get; set; } = "Good";  // Default value using C# 6+ initializer
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;  // Defaults to current UTC time
    public bool IsGood { get; set; } = true;  // Boolean flag, defaults to true
}

// Class to mimic AFDataPipeEvent
// This holds the event details pushed by the pipe
public class DataPipeEvent
{
    public PipeAction Action { get; set; }  // The type of action (from enum above)
    public PipeValue Value { get; set; }    // The data value associated with the event
    public string PointName { get; set; } = "SimulatedPoint";  // Default name for the data point
}

// Interface for the observer (like IObserver<AFDataPipeEvent>)
// Interfaces define contracts (methods that must be implemented)
public interface IDataPipeObserver
{
    void OnNext(DataPipeEvent e);     // Called for each new event
    void OnError(Exception ex);       // Called if an error occurs
    void OnCompleted();               // Called when the stream ends
}

// Custom observer implementation
// This is a generic class: <T> allows it to work with types derived from DataPipeEvent
// ": IDataPipeObserver" means it implements the interface above
// "where T : DataPipeEvent" is a type constraint: It restricts T to be DataPipeEvent or a subclass.
// This ensures type safety—OnNext expects T, but we cast from DataPipeEvent.
public class GenericObserver<T> : IDataPipeObserver where T : DataPipeEvent
{
    // Private fields to store the lambda actions provided in the constructor
    private readonly Action _onCompleted;  // Action is a delegate type (like a function pointer) with no params/return
    private readonly Action<Exception> _onError;  // Action<Exception> takes an Exception param
    private readonly Action<T> _onNext;  // Action<T> takes a T param

    // Constructor: Initializes the observer with three lambdas
    // Constructors are special methods called when creating an instance (new GenericObserver<...>())
    public GenericObserver(Action onCompleted, Action<Exception> onError, Action<T> onNext)
    {
        _onCompleted = onCompleted;
        _onError = onError;
        _onNext = onNext;
    }

    // Implementing interface methods
    // These call the stored lambdas
    public void OnNext(DataPipeEvent e) => _onNext((T)e);  // Cast e to T (safe due to constraint); => is lambda shorthand
    public void OnError(Exception ex) => _onError(ex);
    public void OnCompleted() => _onCompleted();
}

// Simulated DataPipe (observable source)
// This class pushes events to the observer, mimicking a data stream
public class SimulatedDataPipe
{
    private IDataPipeObserver _observer;  // Stores the subscribed observer
    private CancellationTokenSource _cts = new CancellationTokenSource();  // For cancelling the async task
    private bool _isRunning = false;  // Flag to prevent multiple simulations

    // Subscribe method to attach observer
    // This starts the simulation when called
    public void Subscribe(IDataPipeObserver observer)
    {
        _observer = observer;
        StartSimulation();
    }

    // Simulate pushing events over time
    // "async void" for fire-and-forget async method (not ideal in prod, but simple here)
    private async void StartSimulation()
    {
        if (_isRunning) return;  // Prevent re-entry
        _isRunning = true;

        try  // Try-catch for error handling
        {
            int counter = 0;
            while (!_cts.IsCancellationRequested)  // Loop until cancelled
            {
                // Simulate an event
                var e = new DataPipeEvent  // "var" infers type (DataPipeEvent here)
                {
                    Action = (counter % 2 == 0) ? PipeAction.Add : PipeAction.Update,  // Ternary operator: condition ? true : false
                    Value = new PipeValue
                    {
                        Value = counter * 10.0,  // Simulated value (double)
                        Status = "Good",
                        Timestamp = DateTime.UtcNow,
                        IsGood = true
                    }
                };

                _observer?.OnNext(e);  // ? is null-conditional: Calls only if _observer != null
                counter++;

                await Task.Delay(1000);  // Async delay for 1 second; await pauses without blocking thread
            }

            _observer?.OnCompleted();  // Notify completion
        }
        catch (Exception ex)
        {
            _observer?.OnError(ex);  // Notify error
        }
        finally  // Always runs, even after catch
        {
            _isRunning = false;
        }
    }

    // Method to stop the pipe
    public void Stop()
    {
        _cts.Cancel();  // Signals the loop to stop
    }
}

class Program  // Main entry point class
{
    static void Main(string[] args)  // Static method called at startup; args are command-line params
    {
        // Create the simulated pipe
        var dataPipe = new SimulatedDataPipe();

        // Create the observer with lambdas (mimicking your original code)
        // Here, <DataPipeEvent> specifies the generic type T
        var observer = new GenericObserver<DataPipeEvent>(
            () => Console.WriteLine("Simulation completed."),  // OnCompleted lambda (no params)
            ex => Console.WriteLine($"Error: {ex.Message}"),    // OnError lambda (takes Exception); $ for string interpolation
            e =>                                               // OnNext lambda (takes DataPipeEvent)
            {
                // Process the event (emit or log)
                // Multi-line lambda body in { }
                Console.WriteLine($"Event: Action={e.Action}, Point={e.PointName}, Value={e.Value.Value}, Status={e.Value.Status}, Timestamp={e.Value.Timestamp}");
            }
        );

        // Subscribe the observer to the pipe
        dataPipe.Subscribe(observer);

        // Let it run for a bit
        Console.WriteLine("Press Enter to stop the simulation...");
        Console.ReadLine();  // Blocks until user presses Enter

        // Stop the pipe
        dataPipe.Stop();
    }
}
