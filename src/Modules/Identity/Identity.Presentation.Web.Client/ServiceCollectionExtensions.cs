namespace Modules.Identity.Presentation.Web.Client;

using BridgingIT.DevKit.Common;
using Common;
using Common.Presentation.Web.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromApplicationDependencies(a => !a.FullName.StartsWithAny(new[] { "Microsoft", "System", "Scrutor" }))
            .AddClasses(classes => classes.AssignableTo(typeof(IPermissions)), true)
            .AsImplementedInterfaces());

        return services;
    }

    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(sp => sp.GetService<IConfiguration>().GetSection("App").Get<AppConfiguration>());

        return services;
    }
}