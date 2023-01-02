using System.Reflection;
using LearnSpot.Shared.Abstractions.Modules;
using LearnSpot.Shared.Infrastructure;
using LearnSpot.Shared.Infrastructure.Modules;

namespace LearnSpot.Bootstrapper;

public class Startup
{
    private readonly IList<Assembly> _assemblies;
    private readonly IList<IModule> _modules;

    public Startup(IConfiguration configuration)
    {
        _assemblies = ModuleLoader.LoadAssemblies(configuration);
        _modules = ModuleLoader.LoadModules(_assemblies);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure(_assemblies, _modules);
        foreach (var module in _modules)
        {
            module.Register(services);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        logger.LogInformation($"Loaded modules: {string.Join(", " , _modules.Select(x=>x.Name))}");
        app.UserInfrastructure();
        foreach (var module in _modules)
        {
            module.Use(app);
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("LearnSpot API works!"));
            endpoints.MapModuleInfo();
        });
        
        _assemblies.Clear();
        _modules.Clear();
    }
}