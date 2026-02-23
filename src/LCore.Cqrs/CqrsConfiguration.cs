using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lumini.Core.Cqrs;

/// <summary>
/// Provides configuration options for setting up CQRS (Command Query Responsibility Segregation) services within an
/// application.
/// </summary>
/// <remarks>Use this class to add CQRS handlers and decorators to the specified service collection. This enables
/// the application to process commands and queries using the CQRS pattern by registering the necessary types and
/// behaviors.</remarks>
/// <param name="services">The <see cref="IServiceCollection"/> instance used to register CQRS-related services and decorators.</param>
public class CqrsConfiguration(IServiceCollection services)
{
    /// <summary>
    /// Configures CQRS (Command Query Responsibility Segregation) services by registering all command and query
    /// handlers found in the specified assembly.
    /// </summary>
    /// <remarks>Use this method to enable the CQRS pattern in your application by automatically discovering
    /// and registering handlers from the provided assembly.</remarks>
    /// <param name="assembly">The assembly that contains the command and query handler types to be registered. Cannot be null.</param>
    /// <returns>The current instance of the CqrsConfiguration, enabling method chaining.</returns>
    public CqrsConfiguration AddCqrsForAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        services.AddCqrsForAssembly(assembly);

        return this;
    }

    /// <summary>
    /// Adds a decorator of the specified type to the CQRS configuration pipeline.
    /// </summary>
    /// <remarks>Use this method to register a decorator that will be applied to CQRS handlers. The decorator
    /// type must be compatible with the CQRS pipeline and will be resolved by the underlying dependency injection
    /// container.</remarks>
    /// <param name="decoratorType">The type of the decorator to add. This parameter cannot be null.</param>
    /// <returns>The current instance of the CqrsConfiguration, enabling method chaining.</returns>
    public CqrsConfiguration AddDecorator(Type decoratorType)
    {
        ArgumentNullException.ThrowIfNull(decoratorType);

        services.AddDecorator(decoratorType);

        return this;
    }
}