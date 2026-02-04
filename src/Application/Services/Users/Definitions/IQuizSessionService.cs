using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.QuizSessions;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling resource comments.
/// </summary>
public interface IQuizSessionService {

    /// <summary>
    /// Tries to start a quiz session.
    /// </summary>
    /// <returns>The started session.</returns>
    /// <param name="byUserId">The identifier of the user starting the session.</param>
    /// <param name="participants">The identifier of the users allowed to participate.</param>
    /// <param name="quizResourceId">The identifier of the quiz resource used for the session.</param>
    public Task<IResponse<QuizSession>> TryStartAsync(Id byUserId, IEnumerable<Id> participants, Id quizResourceId);

    /// <summary>
    /// Tries to participate to a quiz session.
    /// </summary>
    /// <returns>The updated session details.</returns>
    /// <param name="score">The score of the participation.</param>
    /// <param name="userId">The identifier of the user participating to the session.</param>
    /// <param name="quizSessionId">The identifier of the quiz session.</param>
    public Task<IResponse<QuizSession>> TryParticipateAsync(Id quizSessionId, Id userId, int score);

}
