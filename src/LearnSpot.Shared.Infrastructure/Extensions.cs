using System.Reflection;
using System.Runtime.CompilerServices;
using LearnSpot.Shared.Abstractions.Modules;
using LearnSpot.Shared.Infrastructure.Api;
using LearnSpot.Shared.Infrastructure.Exceptions;
using LearnSpot.Shared.Infrastructure.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("LearnSpot.Bootstrapper")]
namespace LearnSpot.Shared.Infrastructure;

internal static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IList<Assembly> assemblies, IList<IModule> modules)
    {
        var disabledModules = new List<string>();
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            foreach (var (key, value) in configuration.AsEnumerable())
            {
                if (!key.Contains(":module:enabled"))
                {
                    continue;
                }

                if (!bool.Parse(value))
                {
                    disabledModules.Add(key.Split(":")[0]);
                }
            }
        }

        services.AddModuleInfo(modules);
        services.AddErrorHandling();

        services.AddControllers().ConfigureApplicationPartManager(manager =>
        {
            var removedParts = new List<ApplicationPart>();
            foreach (var disabledModule in disabledModules)
            {
                var parts = manager.ApplicationParts.Where(x => x.Name.Contains(disabledModule,
                    StringComparison.InvariantCultureIgnoreCase));
                removedParts.AddRange(parts);
            }

            foreach (var part in removedParts)
            {
                manager.ApplicationParts.Remove(part);
            }

            manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
        });
        
        return services;
    }

    public static IApplicationBuilder UserInfrastructure(this IApplicationBuilder app)
    {
        app.UseErrorHandling();
        app.UseRouting();

        return app;
    }
}