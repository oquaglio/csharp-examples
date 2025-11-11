using System;
// Alias example (outside any namespace)
using AliasForNamespace = NamespaceDemo.InnerNamespace;

namespace NamespaceDemo
{
    // This is a class inside the root namespace 'NamespaceDemo'
    public class Greeting
    {
        public void SayHello()
        {
            Console.WriteLine("Hello from NamespaceDemo.Greeting!");
        }
    }

    // Nested namespace example
    namespace InnerNamespace
    {
        public class Farewell
        {
            public void SayGoodbye()
            {
                Console.WriteLine("Goodbye from NamespaceDemo.InnerNamespace.Farewell!");
            }
        }
    }
}

// Another top-level namespace in the same file (C# allows multiple namespaces per file)
namespace AnotherNamespace
{
    public class Message
    {
        public void DisplayMessage()
        {
            // Using fully qualified name to access a type from another namespace
            NamespaceDemo.Greeting greeting = new NamespaceDemo.Greeting();
            greeting.SayHello();

            // Using a type from the nested namespace
            NamespaceDemo.InnerNamespace.Farewell farewell = new NamespaceDemo.InnerNamespace.Farewell();
            farewell.SayGoodbye();

            Console.WriteLine("Message from AnotherNamespace.Message!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Using types with 'using' directives (assumed at top, but shown here for clarity)
        // In a real project, add 'using NamespaceDemo;' at the top to simplify

        // Fully qualified access
        NamespaceDemo.Greeting greeting = new NamespaceDemo.Greeting();
        greeting.SayHello();

        // Nested namespace access
        NamespaceDemo.InnerNamespace.Farewell farewell = new NamespaceDemo.InnerNamespace.Farewell();
        farewell.SayGoodbye();

        // From another namespace
        AnotherNamespace.Message message = new AnotherNamespace.Message();
        message.DisplayMessage();

        // Using namespace alias
        AliasForNamespace.Farewell aliasedFarewell = new AliasForNamespace.Farewell();
        aliasedFarewell.SayGoodbye();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
