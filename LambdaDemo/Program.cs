// Simple class for demonstration
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // Example 1: Basic Lambda as a Delegate
        // Define a delegate type (optional in modern C#)
        Action<string> greet = message => Console.WriteLine("Hello, " + message);
        greet("World!");  // Output: Hello, World!

        // Example 2: Lambda with Parameters and Return Value
        Func<int, int, int> add = (x, y) => x + y;
        Console.WriteLine("5 + 3 = " + add(5, 3));  // Output: 5 + 3 = 8

        // Example 3: Lambda in LINQ (Querying a Collection)
        List<Person> people = new List<Person>
        {
            new Person { Name = "Alice", Age = 30 },
            new Person { Name = "Bob", Age = 25 },
            new Person { Name = "Charlie", Age = 35 }
        };

        // Use lambda to filter adults (Age >= 30)
        var adults = people.Where(p => p.Age >= 30);
        Console.WriteLine("Adults:");
        foreach (var person in adults)
        {
            Console.WriteLine(person.Name);  // Output: Alice, Charlie
        }

        // Example 4: Lambda with Multiple Statements (Block Body)
        Func<int, string> fizzBuzz = n =>
        {
            if (n % 3 == 0 && n % 5 == 0) return "FizzBuzz";
            if (n % 3 == 0) return "Fizz";
            if (n % 5 == 0) return "Buzz";
            return n.ToString();
        };
        Console.WriteLine("15: " + fizzBuzz(15));  // Output: 15: FizzBuzz

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
