using Lumini.Core.Cqrs.Commands;

namespace Example.Cqrs.Commands;

internal class DoSomethingCommand : ICommand
{
}

internal class DoSomethingCommandHandler : ICommandHandler<DoSomethingCommand>
{
    public async Task Handle(DoSomethingCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine("From Handler -> Doing something...");
    }
}
