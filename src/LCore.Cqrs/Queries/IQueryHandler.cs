using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Queries;

/// <summary>
/// Defines a contract for handling queries of a specified type and returning a result asynchronously.
/// </summary>
/// <remarks>Implementations should provide the logic to process the given query and return the corresponding
/// result. The Handle method supports cancellation via a CancellationToken parameter.</remarks>
/// <typeparam name="TQuery">The type of the query to handle. Must implement the IQuery<TResult> interface.</typeparam>
/// <typeparam name="TResult">The type of the result returned after processing the query.</typeparam>
public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Processes the specified query asynchronously and returns the result of the operation.
    /// </summary>
    /// <remarks>If the operation is canceled via the provided cancellation token, the returned task will be
    /// in the Canceled state. Callers should handle potential cancellation appropriately.</remarks>
    /// <param name="query">The query to process. This parameter provides the data and parameters required to perform the operation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the outcome of processing the query.</returns>
    new Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
