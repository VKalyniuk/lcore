namespace Lumini.Core.Cqrs.Decorators;

/// <summary>
/// Represents a delegate that processes a request asynchronously and returns a response of the specified type.
/// </summary>
/// <remarks>This delegate is commonly used in scenarios where requests are handled asynchronously, allowing for
/// cancellation support via the provided token.</remarks>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
/// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
/// <returns>A task that represents the asynchronous operation. The task result contains the response of type TResponse.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

/// <summary>
/// Defines a contract for decorating request handlers with additional processing logic in a request handling pipeline.
/// </summary>
/// <remarks>Implementations of this interface can be used to add cross-cutting concerns, such as logging,
/// validation, or exception handling, to request processing. Decorators are typically used in a chain of responsibility
/// pattern, allowing multiple behaviors to be composed around a core handler.</remarks>
/// <typeparam name="TRequest">The type of the request to be handled. This type must be non-nullable.</typeparam>
/// <typeparam name="TResponse">The type of the response returned after processing the request.</typeparam>
public interface IHandlerDecorator<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Adds custom behavior to the request handling pipeline by executing logic before or after the request is
    /// processed by the next handler.
    /// </summary>
    /// <remarks>Use this method to implement cross-cutting concerns such as logging, validation, or error
    /// handling within the request processing pipeline. The decorator can perform actions before invoking the next
    /// handler, after it completes, or both.</remarks>
    /// <param name="request">The request message containing the data required for processing.</param>
    /// <param name="next">A delegate that represents the next handler in the pipeline to invoke for processing the request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation of processing the request, which completes when the request
    /// has been handled.</returns>
    Task Decorate(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}
