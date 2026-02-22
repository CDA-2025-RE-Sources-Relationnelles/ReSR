using System.Security.Claims;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Presentation.Api.Users.Authorization;
public class ResourceReadAuthorizationRequirement : IAuthorizationRequirement {}
public class ResourceReadAuthorizationHandler(
    IRepository<Resource> resourceRepository,
    IRepository<User> userRepository
) : AuthorizationHandler<ResourceReadAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext          context,
        ResourceReadAuthorizationRequirement requirement
    ) {
        if (
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["resourceId"]?.ToString(), out var resourceId) &&
            (await resourceRepository.TryGetAsync(resourceId)) is ISuccess<Resource> resource
        ) (
            Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && context.User.IsInRole(nameof(User))
                ? await userRepository
                    .TryGetAsync(userId)
                    .OnSuccessAsync(user => UserPermissionsService.TryVerifyUserResourceAccess(user, resource.Value))
                : resource.Value.Visibility == Visibility.Public
                    ? Response.Success(resource)
                    : Response.Failure()

        ).OnSuccess(() => context.Succeed(requirement))
        .OnFailure(e => context.Fail(new AuthorizationFailureReason(this, e.Message)));

        await Task.CompletedTask;
    }
}