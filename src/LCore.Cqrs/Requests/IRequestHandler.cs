namespace Lumini.Core.Cqrs.Requests;

/// <summary>
/// Defines a contract for handling a request of a specified type asynchronously.
/// </summary>
/// <remarks>Implementations of this interface should provide the logic to process the specified request type. The
/// Handle method is asynchronous and may be invoked concurrently for different requests. Thread safety should be
/// considered in implementations if shared resources are accessed.</remarks>
/// <typeparam name="TRequest">The type of request to be handled. Must implement the IRequest interface.</typeparam>
public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
    /// <summary>
    /// Processes the specified request asynchronously.
    /// </summary>
    /// <remarks>This method is intended to be called in asynchronous workflows. The operation can be
    /// cancelled by providing a cancellation token.</remarks>
    /// <param name="request">The request to be handled. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the request has been processed.</returns>
    Task Handle(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a contract for handling a request of a specified type and returning a response of a specified type
/// asynchronously.
/// </summary>
/// <remarks>Implementations of this interface should provide the logic to process the request and generate the
/// corresponding response. The Handle method is asynchronous and supports cancellation via the provided
/// CancellationToken.</remarks>
/// <typeparam name="TRequest">The type of request to handle. Must implement the IRequest<TResponse> interface.</typeparam>
/// <typeparam name="TResponse">The type of response returned after handling the request.</typeparam>
public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Asynchronously processes the specified request and returns a response.
    /// </summary>
    /// <remarks>This method is intended to be called in asynchronous workflows. Callers should observe the
    /// cancellation token to handle cancellation scenarios appropriately.</remarks>
    /// <param name="request">The request message containing the data required for processing. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response to the request.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
