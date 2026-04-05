using System.Security.Claims;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.AspNetCore.Authorization;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Presentation.Api.Users.Authorization;
public class QuizSessionAuthorizationRequirement : IAuthorizationRequirement {}
public class QuizSessionAuthorizationHandler(
    IRepository<QuizSession> quizSessionRepository,
    IRepository<User> userRepository
) : AuthorizationHandler<QuizSessionAuthorizationRequirement> {

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext         context,
        QuizSessionAuthorizationRequirement requirement
    ) {
        if (
            Id.TryParse((context.Resource as HttpContext)?.Request.RouteValues["quizSessionId"]?.ToString(), out var quizSessionId) &&
            (await quizSessionRepository.TryGetAsync(quizSessionId)) is ISuccess<QuizSession> quizSession
        ) {
            if (
                Id.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) && context.User.IsInRole(nameof(User)) &&
                (await userRepository.TryGetAsync(userId)) is ISuccess<User> user
            ) UserPermissionsService.TryVerifyUserQuizSessionAccess(user.Value, quizSession.Value)
                .OnSuccess(() => context.Succeed(requirement))
                .OnFailure(e => context.Fail(new AuthorizationFailureReason(this, e.Message)));

        } else context.Succeed(requirement);

        await Task.CompletedTask;
    }
}