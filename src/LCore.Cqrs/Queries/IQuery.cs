using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Queries;

/// <summary>
/// Represents the base interface for all query operations within the application.
/// </summary>
/// <remarks>Implement this interface to define a query type that can be handled by a query processor or mediator.
/// This interface provides a consistent contract for queries, enabling a standardized approach to retrieving data or
/// information without modifying system state.</remarks>
public interface IBaseQuery
{
}

/// <summary>
/// Represents a query that returns a result of the specified type.
/// </summary>
/// <remarks>This interface is intended to be implemented by query objects that encapsulate a request for data or
/// computation, returning a result of type TResult. It extends both IBaseQuery and IRequest<TResult> to provide a
/// consistent contract for query handling in applications that use the CQRS (Command Query Responsibility Segregation)
/// pattern.</remarks>
/// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
public interface IQuery<out TResult> : IBaseQuery, IRequest<TResult>
{
}
