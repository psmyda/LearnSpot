using System.Text;
using LearnSpot.Shared.Abstractions.Auth;
using LearnSpot.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LearnSpot.Shared.Infrastructure.Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IList<IModule> modules = null,
        Action<JwtBearerOptions> optionsFactory = null)
    {
        var options = services.GetOptions<AuthOptions>("auth");
        services.AddSingleton<IAuthManager, AuthManager>();

        if (options.AuthenticationDisabled)
        {
            // jakies tam
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.RequireAudience,
            ValidIssuer = options.ValidIssuer,
            ValidIssuers = options.ValidIssuers,
            ValidateActor = options.ValidateActor,
            ValidateAudience = options.ValidateAudience,
            ValidAudiences = options.ValidAudiences,
            ValidateIssuer = options.ValidateIssuer,
            ValidateLifetime = options.ValidateLifetime,
            ValidateTokenReplay = options.ValidateTokenReply,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            SaveSigninToken = options.SaveSigninToken,
            RequireExpirationTime = options.RequireExpirationTime,
            RequireSignedTokens = options.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        if (string.IsNullOrWhiteSpace(options.IssuerSigningKey))
        {
            throw new ArgumentException("Issuer signing key is missing", nameof(options.IssuerSigningKey));
        }

        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.AuthenticationType;
        }

        var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);

        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.RoleClaimType;
        }

        services
            .AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(j =>
            {
                j.Authority = options.Authority;
                j.Audience = options.Audience;
                j.MetadataAddress = options.MetadataAddress;
                j.SaveToken = options.SaveToken;
                j.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
                j.RequireHttpsMetadata = options.RequireHttpsMetadata;
                j.IncludeErrorDetails = options.IncludeErrorDetails;
                j.TokenValidationParameters = tokenValidationParameters;
                if (!string.IsNullOrWhiteSpace(options.Challenge))
                {
                    options.Challenge = options.Challenge;
                }

                optionsFactory?.Invoke(j);
            });

        services.AddSingleton(options);
        services.AddSingleton(tokenValidationParameters);

        var policies = modules?.SelectMany(x => x.Policies ?? Enumerable.Empty<string>()) ??
                       Enumerable.Empty<string>();

        services.AddAuthorization(authorization =>
        {
            foreach (var policy in policies)
            {
                authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
            }
        });

        return services;
    }
}