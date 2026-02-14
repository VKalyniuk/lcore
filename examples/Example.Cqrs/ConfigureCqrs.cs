using Example.Cqrs.Commands;
using Example.Cqrs.Decortors;
using Example.Cqrs.Queries;
using Lumini.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Cqrs;

public static class ConfigureCqrs
{
    public enum ConfigurationMode
    {
        Manual,
        ConfigureByConfigurator,
        AssemblyScanningWithDecorators
    }

    public static IServiceCollection ConfigureCqrsByMode(this IServiceCollection services, ConfigurationMode mode)
    {
        switch (mode)
        {
            case ConfigurationMode.Manual:
                services = ConfigureManual(services);
                break;
            case ConfigurationMode.ConfigureByConfigurator:
                services = ConfigureByConfigurator(services);
                break;
            case ConfigurationMode.AssemblyScanningWithDecorators:
                services = ConfigureAsseblyAndDecorators(services);
                break;
        }

        return services;
    }

    private static IServiceCollection ConfigureByConfigurator(IServiceCollection services)
    {
        services.AddCqrs(config =>
        {
            config.AddCqrsForAssembly(typeof(Program).Assembly);

            config.AddDecorator(typeof(ValidationDecorator<,>));
            config.AddDecorator(typeof(QueryDecorator<,>));
            config.AddDecorator(typeof(CommandDecorator<,>));
        });

        return services;
    }

    private static IServiceCollection ConfigureManual(IServiceCollection services)
    {
        services.AddCqrs();

        services.AddHandler<int, GetIntQuery, GetIntQueryHandler>();
        services.AddHandler<DoSomethingCommand, DoSomethingCommandHandler>();
        services.AddHandler<SaySomethingCommand, SaySomethingCommandHandler>();

        services.AddDecoratorScoped(typeof(ValidationDecorator<,>));
        services.AddDecoratorScoped(typeof(QueryDecorator<,>));
        services.AddDecoratorScoped(typeof(CommandDecorator<,>));

        return services;
    }

    private static IServiceCollection ConfigureAsseblyAndDecorators(IServiceCollection services)
    {
        services.AddCqrsForAssembly(typeof(Program).Assembly);

        services.AddDecoratorScoped(typeof(ValidationDecorator<,>));
        services.AddDecoratorScoped(typeof(QueryDecorator<,>));
        services.AddDecoratorScoped(typeof(CommandDecorator<,>));

        return services;
    }
}
