using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}
