using Lumini.Core.Cqrs.Requests;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Lumini.Core.Cqrs.Senders;

internal class Sender(IServiceProvider serviceProvider) : ISender
{
    private const string HandleMethodName = "Handle";

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

        object handler = GetHandler(handlerType, requestType, responseType: null);

        MethodInfo handleMethod = GetHandleMethod(handler, requestType);

        object result = InvokeAndUnwrap(handler, handleMethod, request, cancellationToken);

        await CastAndValidateTask<Task>(result, handleMethod).ConfigureAwait(false);
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        object handler = GetHandler(handlerType, requestType, typeof(TResponse));

        MethodInfo handleMethod = GetHandleMethod(handler, requestType);

        object result = InvokeAndUnwrap(handler, handleMethod, request, cancellationToken);

        return await CastAndValidateTask<Task<TResponse>>(result, handleMethod).ConfigureAwait(false);
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

    private static object InvokeAndUnwrap(object handler, MethodInfo method, object request, CancellationToken cancellationToken)
    {
        object? result;
        try
        {
            result = method.Invoke(handler, new object[] { request, cancellationToken });
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }

        if (result is null)
        {
            throw new InvalidOperationException(
                $"Invocation of '{method.DeclaringType?.FullName}.{method.Name}' returned null.");
        }

        return result;
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
