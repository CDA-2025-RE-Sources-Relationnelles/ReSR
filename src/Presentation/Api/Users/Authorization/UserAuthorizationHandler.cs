using System.Security.Claims;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Presentation.Api.Users.Authorization;
public class UserAuthorizationRequirement : IAuthorizationRequirement {}
public class UserAuthorizationHandler(
    IRepository<User> repository
) : AuthorizationHandler<UserAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext  context,
        UserAuthorizationRequirement requirement
    ) {

        if (
            Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) &&
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["userId"]?.ToString(), out var parameterId) &&
            userId == parameterId &&
            (await repository.TryGetAsync(userId)) is ISuccess<User> { Value : User { IsAnonymous : false }}
        ) context.Succeed(requirement);

        await Task.CompletedTask;
    }
}