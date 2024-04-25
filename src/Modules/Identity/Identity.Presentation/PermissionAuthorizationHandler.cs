namespace Modules.Identity.Presentation;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Modules.Identity.Application;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;

    public PermissionAuthorizationHandler(IUserService userService) =>
        this._userService = userService;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.FindFirstValue(ClaimTypes.NameIdentifier) is { } userId &&
            (await this._userService.HasPermissionAsync(userId, requirement.Permission)).Value)
        {
            context.Succeed(requirement);
        }
    }
}