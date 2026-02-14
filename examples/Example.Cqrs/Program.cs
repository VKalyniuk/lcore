using Example.Cqrs;
using Example.Cqrs.Commands;
using Example.Cqrs.Queries;
using Lumini.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

bool useAssemblyScanning = true;

if (useAssemblyScanning)
{
    services.AddCqrsForAssembly(typeof(Program).Assembly);
}
else
{
    services.AddCqrs();
    services.AddHandler<int, GetIntQuery, GetIntQueryHandler>();
    services.AddHandler<DoSomethingCommand, DoSomethingCommandHandler>();
    services.AddHandler<SaySomethingCommand, SaySomethingCommandHandler>();
}

services.AddScoped<Examples>();

var example = services.BuildServiceProvider().GetRequiredService<Examples>();

await example.Run();

Console.WriteLine("Finish!");
