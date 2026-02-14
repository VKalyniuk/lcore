using Lumini.Core.Cqrs.Requests;
using Lumini.Core.Cqrs.Senders;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lumini.Core.Cqrs;

public static class CqrsConfiguration
{
    public static IServiceCollection AddCqrsForAssembly(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        services.AddCqrs();

        var requestHandlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
            .Where(x => x.Interface.IsGenericType &&
                        (x.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                         x.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        foreach (var handler in requestHandlerTypes)
        {
            var iface = handler.Interface;
            var openGeneric = iface.GetGenericTypeDefinition();
            var args = iface.GetGenericArguments();
            var serviceType = openGeneric.MakeGenericType(args);

            services.AddScoped(serviceType, handler.Type);
        }

        return services;
    }

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services.AddScoped<ISender, Sender>();

        return services;
    }

    public static IServiceCollection AddHandler<TRequest, THandler>(this IServiceCollection services)
        where TRequest : IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRequestHandler<TRequest>, THandler>();

        return services;
    }

    public static IServiceCollection AddHandler<TResponse, TRequest, THandler>(this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();

        return services;
    }
}
