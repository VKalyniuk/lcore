using Example.Cqrs;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.ConfigureCqrsByMode(ConfigureCqrs.ConfigurationMode.ConfigureByConfigurator);
//services.ConfigureCqrsByMode(ConfigureCqrs.ConfigurationMode.AssemblyScanningWithDecorators);
//services.ConfigureCqrsByMode(ConfigureCqrs.ConfigurationMode.Manual);

services.AddScoped<Examples>();

var example = services.BuildServiceProvider().GetRequiredService<Examples>();

await example.Run();

Console.WriteLine("Finish!");
