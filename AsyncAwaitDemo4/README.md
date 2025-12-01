# CallbackDemo4

## Features Demonstrated

| Feature                  | Why It Matters                                           |
|--------------------------|----------------------------------------------------------|
| Parallel async work      | `Task.WhenAll` → download 5 sites simultaneously       |
| Real I/O (HttpClient)    | Truly non-blocking network calls (no thread waste)      |
| Cancellation             | Gracefully abort long operations via `CancellationToken` |
| Progress reporting       | Real-time feedback with `IProgress<T>`                  |
| Proper error handling    | `try`/`catch` works exactly like synchronous code      |
| Deadlock prevention      | `ConfigureAwait(false)` – essential in libraries/UI    |
| Clean, readable code     | No callback hell, no manual threading                   |

## Why This Beats Old-School Callbacks

| Callbacks (Old Way)           | async/await (Modern Way – This Code)               |
|-------------------------------|----------------------------------------------------|
| Nested, hard-to-read code     | Linear flow – reads like synchronous code         |
| Manual error handling         | Normal `try`/`catch` – just works                  |
| No built-in cancellation      | `CancellationToken` supported everywhere           |
| No progress


## Fire and Forget Pattern

```chsarp
_ = Task.Run(...)   // "Go do this in the background. I don't care when it finishes."
```

If you’re paranoid about exceptions being silently lost you can do somthing like this:
```chsarp
static void FireAndForget(Task task)
{
    task.ContinueWith(t =>
    {
        if (t.IsFaulted)
            Console.WriteLine($"Fire-and-forget task failed: {t.Exception?.Flatten()}");
    }, TaskScheduler.Default);
}

// Usage:
FireAndForget(Task.Run(() => { ... }));
```
