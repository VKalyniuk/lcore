namespace Lumini.Core.Cqrs.Decorators;

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

public interface IHandlerDecorator<TRequest, TResponse> where TRequest : notnull
{
    Task Decorate(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
}