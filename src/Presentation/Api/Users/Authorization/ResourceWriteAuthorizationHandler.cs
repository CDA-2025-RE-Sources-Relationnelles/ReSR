using System.Security.Claims;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Ports;

namespace ReSR.Presentation.Api.Users.Authorization;
public class ResourceWriteAuthorizationRequirement : IAuthorizationRequirement {}
public class ResourceWriteAuthorizationHandler(
    IRepository<Resource> repository
) : AuthorizationHandler<ResourceWriteAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext           context,
        ResourceWriteAuthorizationRequirement requirement
    ) {

        if (
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["resourceId"]?.ToString(), out var resourceId) &&
            (await repository.TryGetAsync(resourceId)) is ISuccess<Resource> resource
        ) {
            if (
                Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) &&
                resource.Value.Owner?.Id == userId
            ) context.Succeed(requirement);

        } else context.Succeed(requirement);

        await Task.CompletedTask;
    }
}