# DataPipeDemo

This demonstrates the push-based, event-driven nature of data pipes without needing the actual PI SDK.

- SimulatedDataPipe: Acts like AFDataPipe, pushing events asynchronously (every 1 second) via OnNext. It supports subscribing an observer and stopping the stream.
- DataPipeEvent & PipeValue: Mimic AFDataPipeEvent and AFValue with basic properties like action, value, status, and timestamp.
- GenericObserver: A wrapper for handling events with lambdas, just like in your code (OnCompleted, OnError, OnNext).
- Main: Sets up the pipe, subscribes the observer, runs for user input, then stops.

