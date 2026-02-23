namespace Lumini.Core.Cqrs.Notifications;

/// <summary>
/// Defines a handler for processing notifications of a specified type.
/// </summary>
/// <remarks>Implement this interface to provide custom logic for handling notifications asynchronously. The
/// Handle method is called when a notification of the specified type is published, allowing for side-effect operations
/// or event-driven processing. Notification handlers are typically used in the context of the mediator pattern to
/// decouple notification publishing from handling logic.</remarks>
/// <typeparam name="TNotification">The type of notification to handle. Must implement the INotification interface.</typeparam>
public interface INotificationHandler<TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the specified notification asynchronously.
    /// </summary>
    /// <remarks>Override this method in a derived class to implement custom notification handling
    /// logic.</remarks>
    /// <param name="notification">The notification to process. This parameter must not be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the notification has been processed.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}
