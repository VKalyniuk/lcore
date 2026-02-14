using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Queries;

public interface IBaseQuery
{
}

public interface IQuery<out TResult> : IBaseQuery, IRequest<TResult>
{
}
