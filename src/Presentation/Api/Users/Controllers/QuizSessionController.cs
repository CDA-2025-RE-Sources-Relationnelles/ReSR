using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Users.Authorization;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.QuizSessions;

namespace ReSR.Presentation.Api.Users.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize]
public class QuizSessionController(
    IQuizSessionService quizSessionService,
    IRepository<QuizSession> repository
) : ControllerBase {

    public const string ROUTE = "/quiz-sessions";
    #region DTOS

        public readonly record struct ParticipateDto(
            int Score
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Queries all the users quiz sessions.")]
        public Task<IResult> GetAllQuizSessionsAsync() =>
            quizSessionService
                .TryGetAllAsync(User.GetUserId()!.Value)
                .ToResourceAsync<QuizSession, QuizSessionResource>(Results.Ok);

        [HttpGet("{quizSessionId}")]
        [Authorize(Policy = nameof(QuizSessionAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the quiz session.")]
        [EndpointDescription("Queries all the users quiz sessions.")]
        public Task<IResult> GetQuizSessionAsync(Id quizSessionId) =>
            repository
                .TryGetAsync(quizSessionId)
                .ToResourceAsync<QuizSession, QuizSessionResource>(Results.Ok);

        [HttpPost("{quizSessionId}/participate")]
        [Authorize(Policy = nameof(QuizSessionAuthorizationRequirement))]
        [EndpointSummary("Only accessible for authenticated users with access to the quiz session.")]
        [EndpointDescription("Starts a session for this quiz resource")]
        public Task<IResult> ParticipateAsync(Id quizSessionId, ParticipateDto dto) =>
            quizSessionService.TryParticipateAsync(
                quizSessionId,
                User.GetUserId()!.Value,
                dto.Score
            ).ToResourceAsync<QuizSession, QuizSessionResource>(Results.Ok);

    #endregion

}
