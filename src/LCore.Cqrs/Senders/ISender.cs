using Lumini.Core.Cqrs.Notifications;
using Lumini.Core.Cqrs.Requests;

namespace Lumini.Core.Cqrs.Senders;

public interface ISender
{
    /// <summary>
    /// Asynchronously sends the specified request for processing.
    /// </summary>
    /// <remarks>Use this method to send a request without blocking the calling thread. If cancellation is
    /// requested via the provided token, the operation will be canceled if possible.</remarks>
    /// <param name="request">The request to be sent. This parameter must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the send operation.</param>
    /// <returns>A task that represents the asynchronous send operation. The task completes when the request has been processed.</returns>
    Task Send(IRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request and asynchronously returns the response from the appropriate handler.
    /// </summary>
    /// <remarks>Use this method to decouple request creation from handling logic, enabling asynchronous and
    /// non-blocking request processing. The operation can be cancelled by providing a cancellation token.</remarks>
    /// <typeparam name="TResponse">The type of the response expected from the request.</typeparam>
    /// <param name="request">The request message to send. Must implement the IRequest<TResponse> interface.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the send operation.</param>
    /// <returns>A task that represents the asynchronous send operation. The task result contains the response of type TResponse.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sends the specified notification.
    /// </summary>
    /// <remarks>Ensure that the notification object is properly initialized before calling this method. The
    /// operation can be cancelled by providing a cancellation token.</remarks>
    /// <param name="notification">The notification to send. Must implement the <see cref="INotification"/> interface and cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the notification operation.</param>
    /// <returns>A task that represents the asynchronous operation of sending the notification.</returns>
    Task Notify(INotification notification, CancellationToken cancellationToken = default);
}
