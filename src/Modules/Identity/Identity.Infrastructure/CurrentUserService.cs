namespace Modules.Identity.Infrastructure;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Modules.Identity.Application;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        EnsureArg.IsNotNull(httpContextAccessor, nameof(httpContextAccessor));

        this.UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        this.Name = httpContextAccessor.HttpContext?.User?.Identity?.Name;
        this.Email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        this.IsAuthenticated = httpContextAccessor.HttpContext?.User?.Identity is not null && httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
        this.Claims = httpContextAccessor.HttpContext?.User?.Claims?.AsEnumerable()?.Select(item => new KeyValuePair<string, string>(item.Type, item.Value))?.ToList();
    }

    public string UserId { get; }

    public string Name { get; }

    public string Email { get; }

    public bool IsAuthenticated { get; }

    public List<KeyValuePair<string, string>> Claims { get; set; }
}