using System;  // Standard .NET namespace for basic types and console I/O
using System.Collections.Generic;  // Standard .NET namespace for List<T>

// Define the IObserver interface (custom, not from any lib)
public interface IObserver
{
    void Update(string message);  // Method called when notified
}

// Define the ISubject interface (custom, not from any lib)
public interface ISubject
{
    void Attach(IObserver observer);    // Add observer
    void Detach(IObserver observer);    // Remove observer
    void Notify(string message);        // Notify all observers
}

// Concrete Subject class (custom implementation)
public class NewsPublisher : ISubject
{
    private List<IObserver> observers = new List<IObserver>();  // Uses generic List from System.Collections.Generic
    private string latestNews;

    public string LatestNews
    {
        get { return latestNews; }
        set
        {
            latestNews = value;
            Notify(latestNews);  // Notify observers on change
        }
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(string message)
    {
        foreach (var observer in observers)
        {
            observer.Update(message);
        }
    }
}

// Concrete Observer class (custom implementation)
public class Subscriber : IObserver
{
    private string name;

    public Subscriber(string name)
    {
        this.name = name;
    }

    public void Update(string message)
    {
        Console.WriteLine($"{name} received news: {message}");
    }
}

class Program  // Entry point class
{
    static void Main(string[] args)  // Standard Main method
    {
        // Create the subject (publisher)
        NewsPublisher publisher = new NewsPublisher();

        // Create observers (subscribers)
        Subscriber alice = new Subscriber("Alice");
        Subscriber bob = new Subscriber("Bob");

        // Attach observers to the subject
        publisher.Attach(alice);
        publisher.Attach(bob);

        // Publish news (this triggers notifications)
        publisher.LatestNews = "Breaking: New AI breakthrough!";

        // Detach one observer
        publisher.Detach(bob);

        // Publish another news (only Alice gets notified)
        publisher.LatestNews = "Update: Markets react positively.";

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
