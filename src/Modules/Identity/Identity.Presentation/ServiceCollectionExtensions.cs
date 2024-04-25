namespace Modules.Identity.Presentation;

using System.Net;
using System.Reflection;
using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Common;
using Common.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Modules.Identity.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IdentityModuleConfiguration configuration)
    {
        EnsureArg.IsNotNull(configuration);

        services
            //.AddCurrentUser()
            .AddPermissions();

        if (configuration.Provider == AuthProvider.AzureAd)
        {
            services.AddAzureAdAuthentication(configuration);
        }
        else
        {
            services.AddJwtAuthentication(configuration);
        }

        return services;
    }

    public static IServiceCollection AddAzureAdAuthentication(
        this IServiceCollection services,
        IdentityModuleConfiguration configuration)
    {
        _ = services
            .AddAuthorization()
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddMicrosoftIdentityWebApi(
                jwtOptions =>
                {
                    jwtOptions.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var principal = context.Principal;
                            var issuer = principal?.GetIssuer();
                            var objectId = principal?.GetObjectId();
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                            if (principal is null || issuer is null || objectId is null)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                                await context.Response.WriteAsync(
                                    System.Text.Json.JsonSerializer.Serialize(Result.Failure("You are not Authorized.")));
                            }

                            // Lookup local user or create one if none exists.
                            var userId = (await userService.CreateUserAsync(principal))?.Value;

                            // We use the nameidentifier claim to store the user id.
                            var identity = principal.Identities.First();
                            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                            identity.TryRemoveClaim(idClaim);
                            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

                            // And the email claim for the email.
                            var upnClaim = principal.FindFirst(ClaimTypes.Upn);
                            if (upnClaim is not null)
                            {
                                var emailClaim = principal.FindFirst(ClaimTypes.Email);
                                identity.TryRemoveClaim(emailClaim);
                                identity.AddClaim(new Claim(ClaimTypes.Email, upnClaim.Value));
                            }

                            // Add the identity roles as claims
                            //var rolesResult = await userService.GetRolesAsync(userId);
                            //if (rolesResult.IsSuccess)
                            //{
                            //    foreach (var role in rolesResult.Value.UserRoles)
                            //    {
                            //        identity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName));
                            //    }
                            //}
                        }
                    };
                },
                identityOptions =>
                {
                    identityOptions.Authority = configuration.AzureAd.Authority;
                    identityOptions.Instance = configuration.AzureAd.Instance;
                    identityOptions.Domain = configuration.AzureAd.Domain;
                    identityOptions.TenantId = configuration.AzureAd.TenantId;
                    identityOptions.ClientId = configuration.AzureAd.ClientId;
                    identityOptions.ClientSecret = configuration.AzureAd.ClientSecret;

                    if (!string.IsNullOrEmpty(configuration.AzureAd.CallbackPath))
                    {
                        identityOptions.CallbackPath = configuration.AzureAd.CallbackPath; //  new PathString("/signin-oidc")
                    }
                    if (!string.IsNullOrEmpty(configuration.AzureAd.SignedOutCallbackPath))
                    {
                        identityOptions.SignedOutCallbackPath = configuration.AzureAd.SignedOutCallbackPath; // new PathString("/signout-callback-oidc")
                    }

                    if (!string.IsNullOrEmpty(configuration.AzureAd.SignUpSignInPolicyId))
                    {
                        identityOptions.SignUpSignInPolicyId = configuration.AzureAd.SignUpSignInPolicyId;
                    }

                    if (!string.IsNullOrEmpty(configuration.AzureAd.SignedOutRedirectUri))
                    {
                        identityOptions.SignedOutRedirectUri = configuration.AzureAd.SignedOutRedirectUri;
                    }

                    foreach (var scope in configuration.AzureAd.Scopes.SafeNull().Split(";"))
                    {
                        identityOptions.Scope.Add(scope);
                    }
                });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IdentityModuleConfiguration configuration)
    {
        EnsureArg.IsNotNullOrEmpty(configuration?.Jwt?.SigningSecret, nameof(IdentityModuleConfiguration.JwtConfiguration.SigningSecret));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((string)configuration.Jwt.SigningSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (!string.IsNullOrEmpty(context.Request.Query["access_token"]) &&
                            context.HttpContext.Request.Path.StartsWithSegments(SignalRHubConstants.HubUrl))
                        {
                            context.Token = context.Request.Query["access_token"]; // hub request: read the token from the query string
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                            return context.Response.WriteAsync(
                                System.Text.Json.JsonSerializer.Serialize(Result.Failure("The Token is expired.")));
                        }
                        else
                        {
                            //#if DEBUG
                            context.NoResult();
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;
                            return context.Response.WriteAsync(context.Exception.ToString());
                            //#else
                            //                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            //                                c.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                            //                                var result = System.Text.Json.JsonSerializer.Serialize(Shared.Result.Fail(localizer["An unhandled error has occurred."]));
                            //                                return c.Response.WriteAsync(result);
                            //#endif
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                            return context.Response.WriteAsync(
                                System.Text.Json.JsonSerializer.Serialize(Result.Failure("You are not Authorized.")));
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json; //System.Net.Mime.MediaTypeNames.Application.Json;
                        return context.Response.WriteAsync(
                            System.Text.Json.JsonSerializer.Serialize(Result.Failure("You are not authorized to access this resource.")));
                    },
                };
            });

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = false;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            };
        });

        return services;
    }

    public static IServiceCollection AddAuthorization(
    this IServiceCollection services,
    IdentityModuleConfiguration configuration)
    {
        // find all permission sets and register them
        services.Scan(scan => scan // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
            .FromApplicationDependencies(a => !a.FullName.StartsWithAny(new[] { "Microsoft", "System", "Scrutor" }))
            .AddClasses(classes => classes.AssignableTo(typeof(IPermissionSet)), true)
            .AsImplementedInterfaces());

        // add the claim policies for all permission sets
        services.AddAuthorization((options, sp) =>
        {
            foreach (var permissionSet in sp.GetServices<IPermissionSet>().SafeNull())
            {
                //logger.LogInformation("identity: add permission policies (set={permissionSet})", permissionSet.GetType().FullName);
                foreach (var prop in permissionSet.GetType().GetNestedTypes()
                    .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim("Permission", propertyValue.ToString()));
                    }
                }
            }
        });

        return services;
    }

    public static IServiceCollection AddAuthorization(
        this IServiceCollection services,
        Action<AuthorizationOptions, IServiceProvider> configureOptions)
    {
        services.AddOptions<AuthorizationOptions>()
                .Configure(configureOptions);

        return services.AddAuthorization();
    }

    private static IServiceCollection AddPermissions(this IServiceCollection services) =>
       services
           .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
           .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
}