namespace Presentation.Web.Client;

using System;
using System.Reflection;
using BridgingIT.DevKit.Common;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Modules.Identity.Presentation.Web.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Console.WriteLine($"**************** {configuration["App:Modules:Identity:Provider"]} *************************");

        services.AddAuthorizationCore((options, sp) => AddPermissionPolicies(options, sp));

        return configuration["App:Modules:Identity:Provider"] switch
        {
            // AzureAd
            "AzureAd" => services
                .AddScoped<IAuthenticationManager, AzureAdAuthenticationManager>()
                .AddScoped<AzureAdAuthorizationMessageHandler>() // FOR HTTPCLIENT
                .AddMsalAuthentication(options =>
                {
                    configuration.Bind("App:Modules:Identity:AzureAd", options.ProviderOptions.Authentication);
                    options.ProviderOptions.DefaultAccessTokenScopes.Add(configuration["App:Modules:Identity:AzureAd:ApiScope"]);
                    options.ProviderOptions.LoginMode = "redirect";
                })
                .AddAccountClaimsPrincipalFactory<AzureAdClaimsPrincipalFactory>()
                .Services,

            // Jwt
            _ => services
                .AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>()
                .AddTransient<IAuthenticationManager, JwtAuthenticationManager>()
                .AddScoped<IAccessTokenProvider, JwtAccessTokenProvider>()
                .AddScoped<JwtAuthenticationRefreshHandler>() // FOR HTTPCLIENT
                .AddScoped<JwtAuthenticationHeaderHandler>()  // FOR HTTPCLIENT
        };
    }

    public static IServiceCollection AddAuthorizationCore(
        this IServiceCollection services,
        Action<AuthorizationOptions, IServiceProvider> configure)
    {
        services.AddPermissions();
        services.AddOptions<AuthorizationOptions>().Configure<IServiceProvider>(configure);
        return services.AddAuthorizationCore();
    }

    private static void AddPermissionPolicies(AuthorizationOptions options, IServiceProvider serviceProvider)
    {
        foreach (var permissions in serviceProvider.GetServices<IPermissions>().SafeNull())
        {
            //Console.WriteLine($"identity: add permission policies (set={permissions.GetType().FullName})");
            foreach (var property in permissions.GetType().GetNestedTypes()
                .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var value = property.GetValue(null);
                if (value is not null)
                {
                    options.AddPolicy(value.ToString(), policy => policy.RequireClaim("permission", value.ToString()));
                }
            }
        }
    }
}