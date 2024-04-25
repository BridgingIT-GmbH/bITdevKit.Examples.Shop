namespace Modules.Identity.Presentation;

using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modules.Identity.Application;
using Modules.Identity.Infrastructure;
using Modules.Identity.Infrastructure.EntityFramework;

public class IdentityModule : WebModuleBase
{
    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<IdentityModuleConfiguration, IdentityModuleConfiguration.Validator>(configuration, this);

        services.AddStartupTasks()
            .WithTask<IdentitySeederTask>(o => o.Enabled().StartupDelay("00:00:05"));

        services.AddSqlServerDbContext<IdentityDbContext>(o => o
                .UseConnectionString(services.BuildServiceProvider().GetRequiredService<IOptions<IdentityModuleConfiguration>>().Value.ConnectionStrings.GetValue("SqlServer"))
                .UseLogger())
            //.WithHealthChecks()
            .WithDatabaseMigratorService();

        services.AddIdentity<AppIdentityUser, AppIdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(moduleConfiguration);
        services.AddAuthorization(moduleConfiguration);

        services.AddHttpContextAccessor();
        // register all the identity services like ICurrentUserService, ITokenService and more... https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
        services.Scan(scan => scan
            .FromAssemblyOf<TokenService>().AddClasses()
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }
}