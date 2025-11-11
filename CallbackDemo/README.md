# CallbackDemo

This is a simple console application demonstrating callbacks in C# using delegates:

- Delegate Definition: CallbackHandler is a delegate that defines the signature of the callback method (takes a string, returns void).
- LongRunningTask Class: Simulates a task with a delay and invokes the provided callback with a result.
- Callback Method: OnTaskCompleted is the method that gets called back, printing the result.
- Main Method: Instantiates the task, creates a delegate for the callback, and passes it to the task method.


