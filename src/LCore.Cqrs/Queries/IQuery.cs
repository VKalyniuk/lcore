using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Queries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}
