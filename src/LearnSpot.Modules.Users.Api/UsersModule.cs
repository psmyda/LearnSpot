using LearnSpot.Modules.Users.Core;
using LearnSpot.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LearnSpot.Modules.Users.Api;

internal class UsersModule : IModule
{
    public const string BasePath = "users";
 
    public string Name { get; } = "Users";
    public string Path => BasePath;
    
    public void Register(IServiceCollection services)
    {
        services.AddCore();
    }

    public void Use(IApplicationBuilder app)
    {
    }
}