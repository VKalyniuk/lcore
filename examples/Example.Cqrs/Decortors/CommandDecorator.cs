using Lumini.Core.Cqrs.Commands;
using Lumini.Core.Cqrs.Decorators;

namespace Example.Cqrs.Decortors;

internal class CommandDecorator<TRequest, TResponse> : IHandlerDecorator<TRequest, TResponse> where TRequest : notnull
{
    public Task Decorate(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        if (request is IBaseCommand)
        {
            Console.WriteLine($"Decorating command of type {request.GetType().Name}");
            var result = next();
            Console.WriteLine($"Finished decorating command of type {request.GetType().Name}");

            return result;
        }

        return next();
    }
}
