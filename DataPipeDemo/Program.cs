// Simple enum to mimic event actions (e.g., Add, Update in AFDataPipe)
public enum PipeAction
{
    Add,
    Update,
    Remove
}

// Simple class to mimic AFValue (data point with value, status, timestamp)
public class PipeValue
{
    public object Value { get; set; }
    public string Status { get; set; } = "Good";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsGood { get; set; } = true;
}

// Class to mimic AFDataPipeEvent
public class DataPipeEvent
{
    public PipeAction Action { get; set; }
    public PipeValue Value { get; set; }
    public string PointName { get; set; } = "SimulatedPoint";
}

// Interface for the observer (like IObserver<AFDataPipeEvent>)
public interface IDataPipeObserver
{
    void OnNext(DataPipeEvent e);
    void OnError(Exception ex);
    void OnCompleted();
}

// Custom observer implementation
public class GenericObserver<T> : IDataPipeObserver where T : DataPipeEvent
{
    private readonly Action _onCompleted;
    private readonly Action<Exception> _onError;
    private readonly Action<T> _onNext;

    public GenericObserver(Action onCompleted, Action<Exception> onError, Action<T> onNext)
    {
        _onCompleted = onCompleted;
        _onError = onError;
        _onNext = onNext;
    }

    public void OnNext(DataPipeEvent e) => _onNext((T)e);
    public void OnError(Exception ex) => _onError(ex);
    public void OnCompleted() => _onCompleted();
}

// Simulated DataPipe (observable source)
public class SimulatedDataPipe
{
    private IDataPipeObserver _observer;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private bool _isRunning = false;

    // Subscribe method to attach observer
    public void Subscribe(IDataPipeObserver observer)
    {
        _observer = observer;
        StartSimulation();
    }

    // Simulate pushing events over time
    private async void StartSimulation()
    {
        if (_isRunning) return;
        _isRunning = true;

        try
        {
            int counter = 0;
            while (!_cts.IsCancellationRequested)
            {
                // Simulate an event
                var e = new DataPipeEvent
                {
                    Action = (counter % 2 == 0) ? PipeAction.Add : PipeAction.Update,
                    Value = new PipeValue
                    {
                        Value = counter * 10.0,  // Simulated value
                        Status = "Good",
                        Timestamp = DateTime.UtcNow,
                        IsGood = true
                    }
                };

                _observer?.OnNext(e);  // Push to observer
                counter++;

                await Task.Delay(1000);  // Simulate 1-second interval
            }

            _observer?.OnCompleted();
        }
        catch (Exception ex)
        {
            _observer?.OnError(ex);
        }
        finally
        {
            _isRunning = false;
        }
    }

    // Method to stop the pipe
    public void Stop()
    {
        _cts.Cancel();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Create the simulated pipe
        var dataPipe = new SimulatedDataPipe();

        // Create the observer with lambdas (mimicking your original code)
        var observer = new GenericObserver<DataPipeEvent>(
            () => Console.WriteLine("Simulation completed."),
            ex => Console.WriteLine($"Error: {ex.Message}"),
            e =>
            {
                // Process the event (emit or log)
                Console.WriteLine($"Event: Action={e.Action}, Point={e.PointName}, Value={e.Value.Value}, Status={e.Value.Status}, Timestamp={e.Value.Timestamp}");
            }
        );

        // Subscribe the observer to the pipe
        dataPipe.Subscribe(observer);

        // Let it run for a bit
        Console.WriteLine("Press Enter to stop the simulation...");
        Console.ReadLine();

        // Stop the pipe
        dataPipe.Stop();
    }
}
