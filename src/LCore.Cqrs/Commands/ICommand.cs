using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Commands;

public  interface IBaseCommand
{
}

public interface ICommand : IBaseCommand, IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}
