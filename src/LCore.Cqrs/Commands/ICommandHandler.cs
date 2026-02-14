using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
{
    new Task Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    new Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
