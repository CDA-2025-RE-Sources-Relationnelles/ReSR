using System.Security.Claims;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Presentation.Api.Managers.Authorization;
public class ManagerSessionAuthorizationRequirement : IAuthorizationRequirement {}
public class ManagerSessionAuthorizationHandler(
    IRepository<User> repository
) : AuthorizationHandler<ManagerSessionAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext     context,
        ManagerSessionAuthorizationRequirement requirement
    ) {

        if (
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["managerId"]?.ToString(), out var parameterId) &&
            (await repository.TryGetAsync(parameterId)) is ISuccess<Manager>
        ) {
            if (
                Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) &&
                userId == parameterId
            ) context.Succeed(requirement);
        } else context.Succeed(requirement);

        await Task.CompletedTask;
    }
}