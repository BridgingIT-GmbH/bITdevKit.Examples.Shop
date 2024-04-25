namespace Microsoft.Extensions.DependencyInjection;

using BridgingIT.DevKit.Application.Collaboration;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailService(
        this IServiceCollection services,
        MailConfiguration configuration)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        configuration ??= new MailConfiguration();

        services.TryAddTransient<IMailService>(sp => new MailService(o => o
            .LoggerFactory(sp.GetRequiredService<ILoggerFactory>())
            .Authentication(configuration.UserName, configuration.Password)
            .Host(configuration.Host, configuration.Port)
            .Sender(configuration.From, configuration.DisplayName)));

        return services;
    }

    public static IServiceCollection AddMailService(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "Mail")
    {
        EnsureArg.IsNotNull(services, nameof(services));

        var mailConfiguration = configuration?.GetSection(section)?.Get<MailConfiguration>() ?? new MailConfiguration();

        services.TryAddTransient<IMailService>(sp => new MailService(o => o
            .LoggerFactory(sp.GetRequiredService<ILoggerFactory>())
            .Authentication(mailConfiguration.UserName, mailConfiguration.Password)
            .Host(mailConfiguration.Host, mailConfiguration.Port)
            .Sender(mailConfiguration.From, mailConfiguration.DisplayName)));

        return services;
    }
}
