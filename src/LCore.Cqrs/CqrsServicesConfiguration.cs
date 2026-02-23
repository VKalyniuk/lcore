using Lumini.Core.Cqrs.Decorators;
using Lumini.Core.Cqrs.Notifications;
using Lumini.Core.Cqrs.Requests;
using Lumini.Core.Cqrs.Senders;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lumini.Core.Cqrs;

/// <summary>
/// Provides extension methods for registering and configuring CQRS (Command Query Responsibility Segregation) services
/// in an ASP.NET Core application's dependency injection container.
/// </summary>
/// <remarks>This static class includes methods to add core CQRS services, register command and query handlers,
/// apply decorators, and automatically discover and register handlers from a specified assembly. These methods are
/// intended to simplify the setup of the CQRS pattern by ensuring that all required services and handlers are properly
/// registered for dependency injection.</remarks>
public static class CqrsServicesConfiguration
{
    /// <summary>
    /// Adds the services required to support the CQRS pattern to the specified dependency injection container.
    /// </summary>
    /// <remarks>This method registers the <see cref="ISender"/> implementation, allowing the application to
    /// send commands and queries using the CQRS pattern.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the CQRS services will be added. This parameter cannot be null.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance, enabling method chaining.</returns>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ISender, Sender>();

        return services;
    }

    /// <summary>
    /// Registers CQRS services in the specified service collection and applies custom configuration options.
    /// </summary>
    /// <remarks>Use this method to add CQRS support to your application's dependency injection container with
    /// custom configuration. The provided configuration action enables you to modify CQRS-related options and service
    /// registrations during setup.</remarks>
    /// <param name="services">The service collection to which CQRS services will be added. Cannot be null.</param>
    /// <param name="configuration">An action that configures CQRS settings, allowing customization of the registered services. Cannot be null.</param>
    /// <returns>The service collection with CQRS services and custom configuration applied.</returns>
    public static IServiceCollection AddCqrs(this IServiceCollection services, Action<CqrsConfiguration> configuration)
    {
        services.AddCqrs();
        configuration(new CqrsConfiguration(services));

        return services;
    }

    /// <summary>
    /// Registers a request handler of the specified type for a given request type in the service collection.
    /// </summary>
    /// <remarks>The handler is registered with scoped lifetime, so a new instance is created for each request
    /// scope.</remarks>
    /// <typeparam name="TRequest">The type of the request to be handled. Must implement the IRequest interface.</typeparam>
    /// <typeparam name="THandler">The type of the handler that processes the request. Must implement IRequestHandler<TRequest>.</typeparam>
    /// <param name="services">The service collection to which the request handler is added. Cannot be null.</param>
    /// <returns>The updated IServiceCollection instance, enabling method chaining.</returns>
    public static IServiceCollection AddHandler<TRequest, THandler>(this IServiceCollection services)
        where TRequest : IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRequestHandler<TRequest>, THandler>();

        return services;
    }

    /// <summary>
    /// Registers a request handler for the specified request and response types in the service collection.
    /// </summary>
    /// <remarks>The handler is registered as a scoped service, so a new instance is created for each
    /// request.</remarks>
    /// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
    /// <typeparam name="TRequest">The type of the request that the handler processes. Must implement IRequest<TResponse>.</typeparam>
    /// <typeparam name="THandler">The type of the handler that processes the request. Must implement IRequestHandler<TRequest, TResponse>.</typeparam>
    /// <param name="services">The service collection to which the request handler is added. Cannot be null.</param>
    /// <returns>The updated service collection, enabling method chaining.</returns>
    public static IServiceCollection AddHandler<TResponse, TRequest, THandler>(this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();

        return services;
    }

    /// <summary>
    /// Registers a handler decorator type with the service collection, enabling the decoration of handler instances
    /// with additional behavior.
    /// </summary>
    /// <remarks>Use this method to apply cross-cutting concerns, such as logging or validation, to handler
    /// instances by registering a decorator type. The decorator will be applied to all handlers resolved from the
    /// service collection.</remarks>
    /// <param name="services">The service collection to which the decorator will be added. This parameter cannot be null.</param>
    /// <param name="decoratorType">The type of the decorator to register. This type must implement the IHandlerDecorator interface and cannot be
    /// null.</param>
    /// <returns>The updated service collection, allowing for method chaining.</returns>
    public static IServiceCollection AddDecorator(this IServiceCollection services, Type decoratorType)
    {
        ArgumentNullException.ThrowIfNull(services, "services");
        ArgumentNullException.ThrowIfNull(decoratorType, "decoratorType");

        services.AddScoped(typeof(IHandlerDecorator<,>), decoratorType);

        return services;
    }

    /// <summary>
    /// Registers CQRS services, including request and notification handlers, from the specified assembly into the
    /// service collection.
    /// </summary>
    /// <remarks>Use this method to enable CQRS functionality by scanning the provided assembly for handler
    /// implementations and registering them with the dependency injection container.</remarks>
    /// <param name="services">The service collection to which the CQRS services will be added. This parameter cannot be null.</param>
    /// <param name="assembly">The assembly containing the request and notification handler types to register. This parameter cannot be null.</param>
    /// <returns>The service collection with the CQRS services and handlers registered.</returns>
    public static IServiceCollection AddCqrsForAssembly(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        services.AddCqrs();

        AddRequestHandlersForAssembly(services, assembly);
        AddNotificationHandlersForAssembly(services, assembly);

        return services;
    }

    /// <summary>
    /// Registers all request handler implementations found in the specified assembly with the provided service
    /// collection.
    /// </summary>
    /// <remarks>This method scans the given assembly for types that implement IRequestHandler<TRequest> or
    /// IRequestHandler<TRequest, TResponse> and registers them with the dependency injection container. Use this method
    /// to enable automatic discovery and registration of CQRS request handlers.</remarks>
    /// <param name="services">The service collection to which the request handlers will be added. Cannot be null.</param>
    /// <param name="assembly">The assembly to scan for types implementing the IRequestHandler interface. Cannot be null.</param>
    /// <returns>The service collection with the discovered request handlers registered.</returns>
    private static IServiceCollection AddRequestHandlersForAssembly(IServiceCollection services, Assembly assembly)
    {
        var handlerGenerics = new[] { typeof(IRequestHandler<>), typeof(IRequestHandler<,>) };
        AddHandlersForAssembly(services, assembly, handlerGenerics);

        return services;
    }

    /// <summary>
    /// Registers all notification handler implementations from the specified assembly with the provided service
    /// collection.
    /// </summary>
    /// <remarks>This method scans the given assembly for types that implement
    /// INotificationHandler<TNotification> and registers them with the service collection. Use this method to enable
    /// MediatR-style notification handling for types defined in external assemblies.</remarks>
    /// <param name="services">The service collection to which notification handlers will be added. Cannot be null.</param>
    /// <param name="assembly">The assembly to scan for types implementing the INotificationHandler interface. Cannot be null.</param>
    /// <returns>The service collection with the discovered notification handlers registered.</returns>
    private static IServiceCollection AddNotificationHandlersForAssembly(IServiceCollection services, Assembly assembly)
    {
        var notitficationGenerics = new[] { typeof(INotificationHandler<>) };
        AddHandlersForAssembly(services, assembly, notitficationGenerics);

        return services;
    }

    private static IServiceCollection AddHandlersForAssembly(IServiceCollection services, Assembly assembly, Type[] generics)
    {
        var handlerTypes = GetTypes(assembly, generics);
        foreach (var handler in handlerTypes)
        {
            var iface = handler.Interface;
            var openGeneric = iface.GetGenericTypeDefinition();
            var args = iface.GetGenericArguments();
            var serviceType = openGeneric.MakeGenericType(args);

            services.AddScoped(serviceType, handler.Type);
        }

        return services;
    }

    private static IEnumerable<TypeInterfacePair> GetTypes(Assembly assembly, Type[] generics)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new TypeInterfacePair { Type = t, Interface = i })
            .Where(x => x.Interface.IsGenericType &&
                        generics.Contains(x.Interface.GetGenericTypeDefinition()));

        return types;
    }

    private class TypeInterfacePair
    {
        public Type Type { get; set; }
        public Type Interface { get; set; }
    }
}
