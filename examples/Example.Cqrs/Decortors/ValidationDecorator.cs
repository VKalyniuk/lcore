using Lumini.Core.Cqrs.Decorators;

namespace Example.Cqrs.Decortors;

internal class ValidationDecorator<TRequest, TResult> : IHandlerDecorator<TRequest, TResult> where TRequest : notnull
{
    public Task Decorate(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken = default)
    {
        var isValid = Validate(request);

        if (isValid)
        {
            return next();
        }
        
        throw new InvalidOperationException("Request validation failed.");
    }

    private bool Validate(TRequest request)
    {
        // Implement your validation logic here
        Console.WriteLine($"Validating request of type {request.GetType().Name}");

        return true;
    }
}
