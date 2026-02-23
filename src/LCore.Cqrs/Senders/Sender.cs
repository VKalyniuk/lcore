using Lumini.Core.Cqrs.Decorators;
using Lumini.Core.Cqrs.Notifications;
using Lumini.Core.Cqrs.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Lumini.Core.Cqrs.Senders;

internal class Sender(IServiceProvider serviceProvider) : ISender
{
    // TODO: Consider caching handler and decorator types for performance optimization, especially in high-throughput scenarios.
    private const string HandleMethodName = "Handle";

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        object handler = GetHandler(handlerType, requestType, responseType: null);

        MethodInfo handleMethod = GetHandleMethod(handler, requestType);

        object result = await InvokeAndUnwrap<Task, IRequest>(handler, handleMethod, request, cancellationToken);

        await CastAndValidateTask<Task>(result, handleMethod).ConfigureAwait(false);
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        object handler = GetHandler(handlerType, requestType, typeof(TResponse));

        MethodInfo handleMethod = GetHandleMethod(handler, requestType);

        object result = await InvokeAndUnwrap<Task<TResponse>, IRequest<TResponse>>(handler, handleMethod, request, cancellationToken);

        return await CastAndValidateTask<Task<TResponse>>(result, handleMethod).ConfigureAwait(false);
    }

    public async Task Notify(INotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        Type notificationType = notification.GetType();
        Type handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        var handlers = GetHandlers(handlerType, notificationType);

        foreach (var handler in handlers)
        {
            MethodInfo handleMethod = GetHandleMethod(handler!, notificationType);
            object result = await InvokeAndUnwrap<Task, INotification>(handler!, handleMethod, notification, cancellationToken);
            await CastAndValidateTask<Task>(result, handleMethod).ConfigureAwait(false);
        }
    }

    private object GetHandler(Type handlerType, Type requestType, Type? responseType)
    {
        object? handler = serviceProvider.GetService(handlerType);

        if (handler is not null)
        {
            return handler;
        }

        if (responseType is null)
        {
            throw new InvalidOperationException(
                $"No handler registered for request '{requestType.FullName}'.");
        }

        throw new InvalidOperationException(
            $"No handler registered for request '{requestType.FullName}' " +
            $"and response '{responseType.FullName}'.");
    }

    private IEnumerable<object?> GetHandlers(Type handlerType, Type notificationType)
    {
        var handlers = serviceProvider.GetServices(handlerType);

        if (handlers is not null)
        {
            return handlers;
        }

        if (handlers!.Any(h => h is null))
        {
            throw new InvalidOperationException(
                $"Not all handlers registered for notification '{notificationType.FullName}'");
        }

        throw new InvalidOperationException(
            $"No handlers registered for notification '{notificationType.FullName}'");
    }

    private static MethodInfo GetHandleMethod(object handler, Type requestType)
    {
        Type runtimeHandlerType = handler.GetType();

        MethodInfo? method = runtimeHandlerType.GetMethod(
            HandleMethodName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: new[] { requestType, typeof(CancellationToken) },
            modifiers: null);

        if (method is null)
        {
            throw new MissingMethodException(
                runtimeHandlerType.FullName,
                $"{HandleMethodName}({requestType.Name}, CancellationToken)");
        }

        return method;
    }

    private async Task<object> InvokeAndUnwrap<TResult, TRequest>(object handler, MethodInfo method, object request, CancellationToken cancellationToken)
        where TRequest : notnull
    {
        try
        {
            RequestHandlerDelegate<TResult> next = (ct) =>
            {
                var result = method.Invoke(handler, new object[] { request, ct })!;
                return Task.FromResult((TResult)result);
            };

            // Wrap 'next' by decorators in reverse order
            var requestType = request.GetType();
            var responseType = typeof(TResult);
            var decoratorType = typeof(IHandlerDecorator<,>).MakeGenericType(requestType, responseType);
            var decorators = serviceProvider.GetServices(decoratorType).Reverse().ToArray();

            foreach (var decorator in decorators)
            {
                var currentNext = next;
                next = (ct) => (Task<TResult>)decoratorType
                    .GetMethod(nameof(IHandlerDecorator<TRequest, TResult>.Decorate))!
                    .Invoke(decorator, new object?[] { request, currentNext, ct })!;
            }

            // Await the delegate result, validate non-null and return as object
            var final = await next(cancellationToken).ConfigureAwait(false);
            if (final is null)
            {
                throw new InvalidOperationException(
                    $"Invocation of '{method.DeclaringType?.FullName}.{method.Name}' returned null.");
            }

            return (object)final;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }

    private static TTask CastAndValidateTask<TTask>(object result, MethodInfo method) where TTask : Task
    {
        Type expectedReturnType = typeof(TTask);

        if (!expectedReturnType.IsAssignableFrom(method.ReturnType))
        {
            throw new InvalidOperationException(
                $"Method '{method.DeclaringType?.FullName}.{method.Name}' " +
                $"must return '{expectedReturnType.FullName}', " +
                $"but returns '{method.ReturnType.FullName}'.");
        }

        if (result is not TTask task)
        {
            throw new InvalidCastException(
                $"Invocation result type '{result.GetType().FullName}' " +
                $"cannot be cast to '{expectedReturnType.FullName}'.");
        }

        return task;
    }
}
