using Lumini.Core.Cqrs.Commands;

namespace Example.Cqrs.Commands;

internal record SaySomethingCommand(string Value) : ICommand;

internal class SaySomethingCommandHandler : ICommandHandler<SaySomethingCommand>
{
    public async Task Handle(SaySomethingCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"From Handler -> Something is '{command.Value}'");
    }
}
