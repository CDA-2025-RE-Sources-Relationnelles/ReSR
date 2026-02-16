using System.Security.Claims;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Presentation.Api.Managers.Authorization;
public class ManagerAuthorizationRequirement : IAuthorizationRequirement {}
public class ManagerAuthorizationHandler(
    IRepository<User> repository
) : AuthorizationHandler<ManagerAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext     context,
        ManagerAuthorizationRequirement requirement
    ) {

        if (
            Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) &&
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["managerId"]?.ToString(), out var parameterId) &&
            userId == parameterId && (await repository.TryGetAsync(userId)) is ISuccess<Manager>
        ) context.Succeed(requirement);

        await Task.CompletedTask;
    }
}