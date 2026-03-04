using System.Security.Claims;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Presentation.Api.Users.Authorization;
public class CommentReadAuthorizationRequirement : IAuthorizationRequirement {}
public class CommentReadAuthorizationHandler(
    IRepository<Comment> commentRepository,
    IRepository<User> userRepository
) : AuthorizationHandler<CommentReadAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext         context,
        CommentReadAuthorizationRequirement requirement
    ) {
        if (
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["commentId"]?.ToString(), out var commentId) &&
            (await commentRepository.TryGetAsync(commentId)) is ISuccess<Comment> comment
        ) (
            Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && context.User.IsInRole(nameof(User))
                ? await userRepository
                    .TryGetAsync(userId)
                    .OnSuccessAsync(user => UserPermissionsService.TryVerifyUserResourceAccess(user, comment.Value.CommentedResource))
                : comment.Value.CommentedResource.Visibility == Visibility.Public
                    ? Response.Success(comment)
                    : Response.Failure()

        ).OnSuccess(() => context.Succeed(requirement))
        .OnFailure(e => context.Fail(new AuthorizationFailureReason(this, e.Message)));
        
        else context.Succeed(requirement);

        await Task.CompletedTask;
    }
}