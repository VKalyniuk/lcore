using Example.Cqrs.Commands;
using Example.Cqrs.Queries;
using Lumini.Core.Cqrs.Senders;
using System.Net.Sockets;

namespace Example.Cqrs;

internal class Examples(ISender sender)
{
    private class Test
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }

    public async Task Run()
    {
        await RunExample(1, Example1);
        await RunExample(2, Example2);
        await RunExample(3, Example3);
    }

    private async Task RunExample(int number, Func<Task> task)
    {
        Console.WriteLine($"Running example {number}...");
        await task();
        Console.WriteLine($"Example {number} completed.");
        Console.WriteLine();
    }

    private async Task Example1()
    {
        var result = await sender.Send(new GetIntQuery());
        Console.WriteLine($"Result: {result}");
    }

    private async Task Example2()
    {
        await sender.Send(new DoSomethingCommand());
    }

    private async Task Example3()
    {
        await sender.Send(new SaySomethingCommand("Hello from example 3"));
    }
}
