using Lumini.Core.Cqrs.Commands;
using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Queries;

public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    new Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
