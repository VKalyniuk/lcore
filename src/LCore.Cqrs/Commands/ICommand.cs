using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Commands;

/// <summary>
/// Defines a marker interface for command types used in command-based operations.
/// </summary>
/// <remarks>Do not implement this interface. It is useful to fing all commanrs or check if request is a Command</remarks>
public interface IBaseCommand
{
}

/// <summary>
/// Represents a command that encapsulates an action to be executed within a command or request handling pattern.
/// </summary>
/// <remarks>This interface extends both IBaseCommand and IRequest, enabling its use in systems that implement
/// command-query responsibility segregation (CQRS). Implementations should define the specific behavior to
/// be performed when the command is executed.</remarks>
public interface ICommand : IBaseCommand, IRequest
{
}

/// <summary>
/// Represents a command that can be executed and returns a result of the specified type.
/// </summary>
/// <remarks>This interface is typically used in the context of the Command pattern, allowing for the
/// encapsulation of a request as an object, thereby enabling parameterization of clients with queues, requests, and
/// operations.</remarks>
/// <typeparam name="TResult">The type of the result returned by the command when executed.</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>
{
}
