using Lumini.Core.Cqrs.Decorators;
using Lumini.Core.Cqrs.Queries;

namespace Example.Cqrs.Decortors;

internal class QueryDecorator<TRequest, TResponse> : IHandlerDecorator<TRequest, TResponse> where TRequest : notnull
{
    public Task Decorate(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        if (request is IBaseQuery)
        {
            Console.WriteLine($"Decorating query of type {request.GetType().Name}");
            next();
            Console.WriteLine($"Finished decorating query of type {request.GetType().Name}");
        }

        return next();
    }
}
