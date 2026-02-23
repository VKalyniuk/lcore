using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Commands;

/// <summary>
/// Defines a handler that processes commands of a specified type.
/// </summary>
/// <remarks>Implementations of this interface should provide the logic for handling the specified command type.
/// The Handle method is invoked to execute the command, and it may be called with a cancellation token to allow for
/// operation cancellation.</remarks>
/// <typeparam name="TCommand">The type of command to be handled. Must implement the ICommand interface.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <remarks>This method is intended to be called in an asynchronous context. If the operation is
    /// canceled, the returned task will be completed in a canceled state.</remarks>
    /// <param name="command">The command to be processed. This parameter must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the command has been processed.</returns>
    new Task Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler that processes a command of a specified type and returns a result asynchronously.
/// </summary>
/// <remarks>Implementations should validate the command and perform the necessary operations to produce a result.
/// This interface extends IRequestHandler<TCommand, TResult> to provide a consistent pattern for command handling in
/// CQRS-based architectures.</remarks>
/// <typeparam name="TCommand">The type of command to be handled. Must implement the ICommand<TResult> interface.</typeparam>
/// <typeparam name="TResult">The type of result returned after processing the command.</typeparam>
public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Processes the specified command asynchronously and returns the result of the operation.
    /// </summary>
    /// <remarks>If the operation is canceled via the provided cancellation token, the returned task will be
    /// in a canceled state. Implementations should honor the cancellation request where possible.</remarks>
    /// <param name="command">The command to be handled. This parameter provides the data required to perform the operation and cannot be
    /// null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the outcome of processing the
    /// command.</returns>
    new Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
