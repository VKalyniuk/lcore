namespace Lumini.Core.Cqrs.Notifications;

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}
